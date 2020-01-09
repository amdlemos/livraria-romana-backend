using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LivrariaRomana.Infrastructure.DBConfiguration
{
    public class DataBaseConnection
    {        
        public static string ConnectionConfiguration
        {
            get
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                    .SetBasePath("E:\\alan\\projetos\\theos sistemas\\LivrariaRomana\\LivrariaRomana.Test")
                    .AddJsonFile("appsettings.json")
                    .Build();
                string connectionString = Configuration.GetConnectionString("DevConnection");
                return connectionString;
            }

        }
    }
}
