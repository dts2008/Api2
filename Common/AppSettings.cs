using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class AppSettings
    {
        private ConfigurationBuilder _builder = new ConfigurationBuilder();

        private IConfigurationRoot _configuration = null;

        private static AppSettings _instance = new AppSettings();

        public static IConfigurationRoot Instance { get => _instance._configuration; }

        private AppSettings()
        {
            _builder.AddJsonFile("appsettings.json", optional: false);
            _configuration = _builder.Build();

            //var connectionString = _configuration.GetConnectionString("MySQLConnection");
        }
    }
}
