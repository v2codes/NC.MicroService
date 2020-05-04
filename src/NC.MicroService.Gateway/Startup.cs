using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NC.MicroService.Gateway.OcelotExtension;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

namespace NC.MicroService.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 1. 注册网关 Ocelot 到IOC容器
            //services.AddOcelot(); // 注册 Ocelot

            //services.AddOcelot().AddConsul(); // 注册 Ocelot，结合 Consul，实现动态路由

            services.AddOcelot().AddConsul().AddPolly(); // 注册 Ocelot，结合 Consul 实现动态路由，结合 Polly 实现熔断/降级。
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 2、使用网关
            // app.UseOcelot().Wait();

            //// 2.1 自定义ocelot中间件注册完成
            app.UseOcelot((build, config) =>
            {
                build.BuildCustomeOcelotPipeline(config);
            }).Wait();


            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
