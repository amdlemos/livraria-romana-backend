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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static void Injection(this IServiceCollection services, IConfiguration configuration)
        {
            IConfiguration dbConnectionSettings = ResolveConfiguration.GetConnectionSettings(configuration);
            string conn = dbConnectionSettings.GetConnectionString("DevConnection");
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(conn));

            services.AddSingleton<ILoggerManager, LoggerManager>();

            // Repositories
            services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
            services.AddScoped(typeof(IDomainRepository<>), typeof(DomainRepository<>));
            services.AddScoped(typeof(IBookRepository), typeof(BookRepository));
            services.AddScoped(typeof(Interfaces.Repositories.Domain.IUserService), typeof(UserRepository));

            // Services
            services.AddScoped(typeof(Interfaces.Services.Domain.IUserService), typeof(UserService));
            services.AddScoped(typeof(IBookService), typeof(BookService));

        }
    }
}
