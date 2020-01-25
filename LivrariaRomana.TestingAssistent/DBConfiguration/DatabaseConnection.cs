using Microsoft.Extensions.Configuration;
using System.IO;

namespace LivrariaRomana.Test.Helper
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
                    .AddJsonFile("appsettings.json")
                    .Build();
                return configuration;// Options.Create(Configuration.GetSection("ConnectionStrings").Get<DataOptionFactory>());
            }
        }
    }
}
