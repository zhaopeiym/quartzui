using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                           .AddJsonFile("Hosting.json")
                                           .Build();
            string url = configuration["server.urls"];
            CreateWebHostBuilder(args).UseUrls(url).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>();
        }
    }
}
