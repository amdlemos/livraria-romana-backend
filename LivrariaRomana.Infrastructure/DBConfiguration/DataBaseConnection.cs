using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LivrariaRomana.Infrastructure.DBConfiguration
{
    public class DatabaseConnection
    {        
        public static IConfiguration ConnectionConfiguration
        {
            get
            {                
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                return configuration;
            }

        }
    }
}
