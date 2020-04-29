using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NC.MicroService.TeamService.EntityFrameworkCore;
using NC.MicroService.TeamService.Repositories;
using NC.MicroService.TeamService.Services;

namespace NC.MicroService.TeamService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // 1. 注册数据库上下文
            services.AddDbContext<CoreContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });

            // 2. 注册团队service
            services.AddScoped<ITeamService, Services.TeamService>();

            // 3. 注册团队仓储
            services.AddScoped<ITeamRepository, TeamRepository>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // 注册Consul
            // 1. 创建consul客户端连接
            var consulClient = new ConsulClient(config =>
            {
                // 建立客户端和服务端连接
                config.Address = new Uri("https://127.0.0.1:8500");
            });

            // 2. 创建consul服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),
                Name = "teamservice",
                Address = "https://localhost",
                Port = 5004,
                Check = new AgentServiceCheck
                {
                    // 3.1、consul健康检查超时间
                    Timeout = TimeSpan.FromSeconds(10),
                    // 3.2、服务停止5秒后注销服务
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    // 3.3、consul健康检查地址
                    HTTP = "https://localhost:5004/HealthCheck",
                    // 3.4 consul健康检查间隔时间
                    Interval = TimeSpan.FromSeconds(3),
                }
            };

            // 3. 注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
