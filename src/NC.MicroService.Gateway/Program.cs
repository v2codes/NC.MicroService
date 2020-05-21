using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;

namespace NC.MicroService.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        // 1. 加载 ocelot 配置文件
                        //config.AddJsonFile("ocelot.json");
                        // 2. 多路由配置
                        config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                               .AddJsonFile("appsettings.json", true, true)
                               .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                               .AddOcelot(hostingContext.HostingEnvironment)
                               .AddEnvironmentVariables();
                    });
                });
    }
}
