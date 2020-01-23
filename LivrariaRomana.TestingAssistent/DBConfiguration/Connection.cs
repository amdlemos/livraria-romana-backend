using LivrariaRomana.Infrastructure.DBConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;

namespace LivrariaRomana.TestingAssistent.DBConfiguration
{
    public class Connection
    {
        private IServiceProvider _provider;

        public DatabaseContext DatabaseConfiguration()
        {            
            var services = new ServiceCollection();
            var connectionString = DatabaseConnection.ConnectionConfiguration.GetConnectionString("DevConnection");
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));
            _provider = services.BuildServiceProvider();
            return _provider.GetService<DatabaseContext>();
        }
    }
}
