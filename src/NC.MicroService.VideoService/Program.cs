using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Winton.Extensions.Configuration.Consul;

namespace NC.MicroService.VideoService
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
                    /********** 启用配置中心 **********/
                    // 1. 动态加载配置中心的配置文件
                    webBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
                    {
                        // 加载默认配置信息到 Configuration 对象
                        hostingContext.Configuration = configBuilder.Build();

                        // 加载Consul配置中心的配置数据
                        string consulUrl = hostingContext.Configuration["Consul_Url"];
                        Console.WriteLine("Consul_Url：{0}", consulUrl);

                        // 动态加载环境信息，主要在于动态获取服务名称和环境变量名称
                        var env = hostingContext.HostingEnvironment;
                        configBuilder
                            // --> 加载环境配置文件
                            .AddConsul(
                                // $"appsettings.json", // --> 单服务单配置，获取方式：Configuration["Leo-Test"]
                                // $"{env.ApplicationName}/appsettings.json", // --> 多服务单配置使用方式
                                $"{env.ApplicationName}/appsettings.{env.EnvironmentName}.json", // 多服务多配置使用方式
                                options =>
                                {
                                    // 设置 consul 地址
                                    options.ConsulConfigurationOptions = ccOptions => { ccOptions.Address = new Uri(consulUrl); };
                                    // 设置配置是否可选
                                    options.Optional = true;
                                    // 设置配置文件更新后是否重新加载
                                    options.ReloadOnChange = true;
                                    // 设置忽略异常
                                    options.OnLoadException = exContext => { exContext.Ignore = false; };
                                }
                            )
                            // --> 加载自定义配置文件 
                            .AddConsul(
                                $"{env.ApplicationName}.custom.json",
                                options =>
                                {
                                    // 设置 consul 地址
                                    options.ConsulConfigurationOptions = ccOptions => { ccOptions.Address = new Uri(consulUrl); };
                                    // 设置配置是否可选 --> 是否要加载的意思？？？
                                    options.Optional = true;
                                    // 设置配置文件更新后是否重新加载
                                    options.ReloadOnChange = true;
                                    // 设置忽略异常
                                    options.OnLoadException = exContext => { exContext.Ignore = true; };
                                }
                            )
                            // --> 加载通用配置文件
                            .AddConsul(
                                $"common.json",
                                options =>
                                {
                                    // 设置 consul 地址
                                    options.ConsulConfigurationOptions = ccOptions => { ccOptions.Address = new Uri(consulUrl); };
                                    // 设置配置是否可选
                                    options.Optional = true;
                                    // 设置配置文件更新后是否重新加载
                                    options.ReloadOnChange = true;
                                    // 设置忽略异常
                                    options.OnLoadException = exContext => { exContext.Ignore = true; };
                                }
                            );
                        // Consul 中加载的配置信息加载到 Configuration 对象，然后通过 Configuration 对象加载到项目中
                        hostingContext.Configuration = configBuilder.Build();
                    });


                    webBuilder.UseStartup<Startup>();
                });
    }
}
