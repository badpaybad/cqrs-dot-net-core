using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace IotHub.Core
{
    public static class ConfigurationManagerExtensions
    {
        public static IConfiguration Configuration { get; private set; }

        public static void SetConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string GetConnectionString(string name)
        {
            if (Configuration == null)
            {
                var consoleFileApp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appSettings.json");

                var builder = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
               ;
                if (File.Exists(consoleFileApp))
                {
                    builder.AddJsonFile(consoleFileApp);
                }

                Configuration = builder.Build();
            }

            return Configuration[$"ConnectionStrings:{name}"];
        }
    }
}
