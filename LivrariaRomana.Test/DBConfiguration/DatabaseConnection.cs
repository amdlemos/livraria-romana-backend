using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LivrariaRomana.Test.DBConfiguration
{
    public class DatabaseConnection
    {
        public static IConfiguration ConnectionConfiguration
        {
            get
            {
                var path = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.ToString()).FullName.ToString();                
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(path)
                    .AddJsonFile("appsettings.test.json")
                    .Build();
                return configuration;// Options.Create(Configuration.GetSection("ConnectionStrings").Get<DataOptionFactory>());
            }
        }
    }
}
