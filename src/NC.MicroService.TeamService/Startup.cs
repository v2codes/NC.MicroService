using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.Infrastructure.Consul;
using NC.MicroService.TeamService.Domain;
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
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));// AiConnection DefaultConnection
            });

            // 2. 注册团队service
            services.AddScoped<ITeamService, Services.TeamService>();

            // 3. 注册团队仓储
            services.AddScoped<ITeamRepository, TeamRepository>();

            // 4. 注册映射
            // services.AddAutoMapper();

            // 5. 注册Consul注册服务
            services.AddConsulRegistry(Configuration);

            // 6、校验AccessToken,从身份校验中心进行校验
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = "http://192.168.2.102:5005"; // 1、授权中心地址
                        options.ApiName = "TeamService"; // 2、api名称(项目具体名称)
                        options.RequireHttpsMetadata = false; // 3、https元数据，不需要
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

            app.UseHttpsRedirection();

            // 1. Consul服务注册
            app.UseConsulRegistry();

            app.UseRouting();

            // 1. 开启身份验证
            app.UseAuthentication();

            // 2. 使用授权
            app.UseAuthorization(); 

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
