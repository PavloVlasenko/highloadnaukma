using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HighLoad
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder<Program>(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder<T>(string[] args) where T : class =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(builder =>
                    {
                        builder.AddUserSecrets<T>();
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}