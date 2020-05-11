using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NC.MicroService.IdentityClient
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
            // 1. 添加身份验证
            // 我们使用cookie来本地登录用户（通过Cookies作为DefaultScheme），并且将 DefaultChallengeScheme设置为oidc。
            // 因为当我们需要用户登录时，我们将使用 OpenId Connect 协议
            services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = "Cookies";
                        options.DefaultChallengeScheme = "oidc"; // openid connect
                    })
                    .AddCookie("Cookies") // 添加可以处理Cookie 的处理程序
                    .AddOpenIdConnect("oidc", options =>
                    {
                        // 1. 生成id_token
                        options.Authority = "http://192.168.2.102:5005";    // 受信任令牌服务地址
                        options.RequireHttpsMetadata = false;
                        options.ClientId = "client-code";
                        options.ClientSecret = "secret";
                        options.ResponseType = "code";
                        options.SaveTokens = true;  // 用于将来自IdentityServer的令牌保留在cookie中

                        // 1. 添加授权访问api的支持(access_token)
                        options.Scope.Add("TeamService");
                        options.Scope.Add("offline_access");
                    });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            // 使用静态文件
            app.UseStaticFiles();

            app.UseRouting();

            // 1. 添加身份认证
            app.UseAuthentication(); 

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"); // .RequireAuthorization();
            });
        }
    }
}
