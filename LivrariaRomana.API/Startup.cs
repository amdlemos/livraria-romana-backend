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

namespace LivrariaRomana.API
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            // Define onde os logs serão criados.
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            
            // 
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
            // Adiciona politicas de Autenticação e Autorização
            //
            #region AUTENICAÇÃO E AUTORIZAÇÃO            
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Adiciono dois níveis de permissão    
            services.AddAuthorization(options =>
            {
                options.AddPolicy("user", policy => policy.RequireClaim("Livraria", "user"));
                options.AddPolicy("admin", policy => policy.RequireClaim("Livraria", "admin"));
            });

            // Diz que a aplicação tem uma autenticação e o default é o JwtBeater
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Diz o formato do Token e como validar o mesmo   
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            #endregion

            //
            // Define consultas e urls minúsculo
            //
            #region ROUTES
            services.AddRouting(o =>
            {
                o.LowercaseUrls = true;
                o.LowercaseQueryStrings = true;                
            });
            #endregion           

            //
            // Adiciona Swagger
            //
            #region SWAGGER            
            services.AddSwaggerGen(x => { x.SwaggerDoc("v1", new OpenApiInfo { Title = "Livraria Romana", Version = "v1" }); });
            #endregion

            //
            // Adiciona politicas do Cors
            //
            #region CORS            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();                    
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

            // 
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Livraria Romana V1");
                // Deixa a interface do usuário Swagger na raiz do aplicativo.
                s.RoutePrefix = string.Empty;
            });

            // Dá permição para minha app Angular
            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
