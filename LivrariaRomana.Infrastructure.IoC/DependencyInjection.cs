using AutoMapper;
using LivrariaRomana.Domain.DTO;
using LivrariaRomana.Domain.Entities;
using LivrariaRomana.Infrastructure.DBConfiguration;
using LivrariaRomana.Infrastructure.Interfaces.Logger;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Domain;
using LivrariaRomana.Infrastructure.Interfaces.Repositories.Standard;
using LivrariaRomana.Infrastructure.Interfaces.Services.Domain;
using LivrariaRomana.Infrastructure.Logger;
using LivrariaRomana.Infrastructure.Repositories.Domain;
using LivrariaRomana.Infrastructure.Repositories.Standard;
using LivrariaRomana.Infrastructure.Services.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

using Microsoft.IdentityModel.Tokens;


namespace LivrariaRomana.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static void Injection(this IServiceCollection services, IConfiguration configuration)
        {
            // DBContext
            IConfiguration dbConnectionSettings = ResolveConfiguration.GetConnectionSettings(configuration);
            string conn = dbConnectionSettings.GetConnectionString("DevConnection");
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(conn));

            // Logger
            services.AddSingleton<ILoggerManager, LoggerManager>();

            // AutoMapper
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Book, BookDTO>();
                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<BookDTO, Book>();
                cfg.CreateMap<User, UserDTO>();
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            // Repositories
            services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddScoped(typeof(IDomainRepository<>), typeof(DomainRepository<>));
            services.AddScoped(typeof(IBookRepository), typeof(BookRepository));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

            // Services
            services.AddScoped(typeof(IUserService), typeof(UserService));
            services.AddScoped(typeof(IBookService), typeof(BookService));

        }
    }
}
