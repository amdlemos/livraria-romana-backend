using LivrariaRomana.Infrastructure.DBConfiguration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Infrastructure.IoC
{
    internal class ResolveConfiguration
    {
        public static IConfiguration GetConnectionSettings(IConfiguration configuration)
        {
            return configuration ?? DatabaseConnection.ConnectionConfiguration;
        }
    }
}
