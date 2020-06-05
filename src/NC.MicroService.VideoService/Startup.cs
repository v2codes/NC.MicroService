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
using NC.MicroService.Infrastructure.Consul;
using NC.MicroService.VideoService.Services;
using NC.MicroService.VideoService.Repositories;
using NC.MicroService.VideoService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NC.MicroService.VideoService
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

            // 1. 注册数据库上下文
            services.AddDbContext<CoreContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));// AiConnection DefaultConnection
            });

            // 2. 注册团队service
            services.AddScoped<IVideoService, Services.VideoService>();

            // 3. 注册团队仓储
            services.AddScoped<IVideoRepository, VideoRepository>();

            // 4. 注册映射
            // services.AddAutoMapper();

            // 5. 注册Consul注册服务
            services.AddConsulRegistry(Configuration);

            //// 6.校验AccessToken,从身份校验中心进行校验-- > Ocelot 网关集成授权认证
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //        .AddIdentityServerAuthentication(options =>
            //        {
            //            options.Authority = "https://192.168.2.105:5005"; // 1. 授权中心地址
            //            options.ApiName = "VideoService"; // 2. api名称(项目具体名称)
            //            options.RequireHttpsMetadata = true; // 3. https元数据，不需要
            //            options.JwtBackChannelHandler = GetHandler(); // 4. 自定义 HttpClientHandler 
            //        });

            //// 7. 注册Saga分布式事务
            //services.AddOmegaCore(options =>
            //{
            //    options.GrpcServerAddress = "192.168.75.148:8080"; // 7.1 协调中心地址
            //    options.InstanceId = "VideoService-ID"; // 7.2 服务实例ID -- 用于集群
            //    options.ServiceName = "VideoService"; // 7.3 服务名称
            //});

            // 8. 添加事件总线 CAP
            services.AddCap(options =>
            {
                // 8.1 使用内存存储消息（消息发送失败处理）
                //options.UseInMemoryStorage();
                // 使用EntityFramework进行存储操作
                options.UseEntityFramework<CoreContext>();
                // 使用MySql进行事务处理
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));// AiConnection DefaultConnection

                // 8.2 使用RabbitMQ进行事件中心处理
                options.UseRabbitMQ(options =>
                {
                    options.HostName = "LL2019";
                    options.UserName = "mq";
                    options.Password = "123456";
                    options.Port = 5672;
                    options.VirtualHost = "/";
                });

                // 8.3 配置定时器尽早启动
                // options.FailedRetryInterval = 2;
                options.FailedRetryCount = 5; // 3次失败 3分钟

                // 8.4 添加CAP后台监控页面（人工处理）
                options.UseDashboard();
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 1. Consul服务注册
            app.UseConsulRegistry();

            app.UseRouting();

            // 1. 开启身份验证 --> Ocelot 网关集成授权认证
            //app.UseAuthentication();

            // 2. 使用授权
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
