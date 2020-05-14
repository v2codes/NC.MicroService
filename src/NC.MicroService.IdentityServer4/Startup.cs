using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NC.MicroService.IdentityServer4.DbContext;

namespace NC.MicroService.IdentityServer4
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
            // 1. IOC容器中添加 IdentityServer4
            services.AddIdentityServer()
                    .AddDeveloperSigningCredential() // 用户登录配置，生产环境需要配置具体登录签名凭据
                    .AddInMemoryApiResources(IdentityServer4Config.GetApiResources()) // 存储API资源，此处使用了测试数据，
                    .AddInMemoryClients(IdentityServer4Config.GetClients())// 存储客户端（模式），配置允许访问IdentityServer4的客户端
                    .AddTestUsers(IdentityServer4Config.GetUsers()) // 客户端用户
                    .AddInMemoryIdentityResources(IdentityServer4Config.Ids); // openid 身份

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
            
            //System.Net.ServicePointManager.Expect100Continue = true;
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls
            //| System.Net.SecurityProtocolType.Tls11
            //| System.Net.SecurityProtocolType.Tls12;

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // 1. 使用 IdentityServer4
            app.UseIdentityServer();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        // 1. 将config中数据存储起来
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in IdentityServer4Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    /*foreach (var resource in Config.Ids)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }*/
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in IdentityServer4Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

        // 2. 将用户中数据存储起来
        private void InitializeUserDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<IdentityServerContext>();
                context.Database.Migrate();

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var idnetityUser = userManager.FindByNameAsync("tony").Result;
                if (idnetityUser == null)
                {
                    idnetityUser = new IdentityUser
                    {
                        UserName = "zhangsan",
                        Email = "zhangsan@email.com"
                    };
                    var result = userManager.CreateAsync(idnetityUser, "123456").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    result = userManager.AddClaimsAsync(idnetityUser, new Claim[] {
                        new Claim(JwtClaimTypes.Name, "leo"),
                        new Claim(JwtClaimTypes.GivenName, "leo"),
                        new Claim(JwtClaimTypes.FamilyName, "leo"),
                        new Claim(JwtClaimTypes.Email, "leo@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://leo.com")
                    }).Result;

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                }
            }
        }
    }
}
