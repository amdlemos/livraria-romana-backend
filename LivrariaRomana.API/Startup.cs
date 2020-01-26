using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using System.Text;
using System.IO;
using LivrariaRomana.Infrastructure.IoC;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;

namespace LivrariaRomana.API
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            // Define onde os logs serão criados.
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //
            // Injeta as dependências necessárias
            //
            #region INJEÇÃO DE DEPENDÊNCIA (Inclusive DBContext)        
            services.Injection(_configuration);
            #endregion

            //
            // Adiciona Cors e MVC
            //
            #region CORS E MVC           
            services.AddCors();
            services.AddMvc(mvcOptions =>
            {
                var authorizationPolice = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                mvcOptions.Filters.Add(new AuthorizeFilter(authorizationPolice));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            #endregion

            //
            // Adiciona politicas de Autenticação e Autorização
            //
            #region AUTENICAÇÃO E AUTORIZAÇÃO
            // Diz que a aplicação tem uma autenticação e o default é o JwtBeater
            // Adiciono dois níveis de permissão    
            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy("admin", authorizationPolicyBuilder => authorizationPolicyBuilder.RequireClaim("bookStore", "admin"));
            });

            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(authenticationOptions =>
            {
                authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Diz o formato do Token e como validar o mesmo   
            .AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.RequireHttpsMetadata = false;
                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            services.AddMvc(mvcOptions =>
            {
                var authorizationPolicy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                mvcOptions.Filters.Add(new AuthorizeFilter(authorizationPolicy));
                mvcOptions.Filters.Add(new AuthorizeFilter(authorizationPolicy));
            })
           .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            #endregion

            //
            // Define consultas e urls minúsculo
            //
            #region ROUTES
            services.AddRouting(routeOptions =>
            {
                routeOptions.LowercaseUrls = true;
                routeOptions.LowercaseQueryStrings = true;
            });
            #endregion           

            //
            // Adiciona Swagger
            //
            #region SWAGGER            
            services.AddSwaggerGen(swaggerGenOptions =>
                {
                    swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "Livraria Romana", Version = "v1" });
                    swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {

                        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });
                    swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
                });
            #endregion




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(swaggerUIoptions =>
            {
                swaggerUIoptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Livraria Romana V1");
                swaggerUIoptions.RoutePrefix = string.Empty;
            });

            app.UseCors(corsPolicyBuilder => corsPolicyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
