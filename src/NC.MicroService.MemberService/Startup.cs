using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NC.MicroService.Infrastructure.Consul;
using NC.MicroService.MemberService.EntityFrameworkCore;
using NC.MicroService.MemberService.Repositories;
using NC.MicroService.MemberService.Services;
using Servicecomb.Saga.Omega.AspNetCore.Extensions;

namespace NC.MicroService.MemberService
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

            // 2. 注册成员Service
            services.AddScoped<IMemberService, Services.MemberService>();

            // 3. 注册成员仓储
            services.AddScoped<IMemberRepository, MemberRepository>();

            // 4. 添加映射
            //services.AddAutoMapper();

            // 5. 添加服务注册条件
            services.AddConsulRegistry(Configuration);

            // 6. 校验AccessToken,从身份校验中心进行校验 --> 参加 TeamService

            // 7. 注册Saga分布式事务
            services.AddOmegaCore(options =>
            {
                options.GrpcServerAddress = "192.168.238.237:8080"; // 7.1 协调中心地址 alpha
                options.InstanceId = "MemberService-ID"; // 7.2 服务实例ID -- 用于集群
                options.ServiceName = "MemberService"; // 7.3 服务名称
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
