using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NC.MicroService.IdentityServer4.DbContext;
using NC.MicroService.IdentityServer4.Models;

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
            #region 内存存储，用于简单功能性测试
            //// 1. IOC容器中添加 IdentityServer4
            //services.AddIdentityServer()
            //        .AddDeveloperSigningCredential() // 用户登录配置，生产环境需要配置具体登录签名凭据
            //         // 测试数据，内存存储
            //        .AddInMemoryApiResources(IdentityServer4Config.GetApiResources()) // 存储API资源，此处使用了测试数据，
            //        .AddInMemoryClients(IdentityServer4Config.GetClients())// 存储客户端（模式），配置允许访问IdentityServer4的客户端
            //        .AddTestUsers(IdentityServer4Config.GetUsers()) // 客户端用户，测试数据
            //        .AddInMemoryIdentityResources(IdentityServer4Config.Ids); // openid 身份资源，测试数据
            #endregion

            #region 将Config配置持久化
            // 1. 
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = Configuration.GetConnectionString("DefaultConnection");// AiConnection DefaultConnection
            services.AddIdentityServer()
                    //表：api/client/identity前缀的 
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                        {
                            builder.UseMySql(connectionString, options =>
                            {
                                options.MigrationsAssembly(migrationsAssembly);
                            });
                        };
                    })
                    //表： devicecodes persistedgrants
                    .AddOperationalStore(options => 
                    {
                        options.ConfigureDbContext = builder =>
                        {
                            builder.UseMySql(connectionString, options =>
                            {
                                options.MigrationsAssembly(migrationsAssembly);
                            });
                        };
                    })
                    //.AddTestUsers(IdentityServer4Config.GetUsers())  // 同样，用户数据也需要改为持久化
                    .AddDeveloperSigningCredential();
            #endregion

            #region 用户数据持久化
            // 2. 注入数据库上下文、
            services.AddDbContext<IdentityServerDbContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));// AiConnection DefaultConnection
            });

            // 用户角色配置，表：aspnet前缀
            services.AddIdentity<User, Role>(options =>
            {
                // 3.1 密码复杂度配置
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<IdentityServerDbContext>()
            .AddDefaultTokenProviders(); // 默认token提供程序，也可以不加
            #endregion

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 初始化配置数据数据
            //InitializeDatabase(app);
            // 初始化用户数据
            InitializeUserDatabase(app);

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

        #region 初始化测试数据：客户端、API资源、测试用户

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
                var context = serviceScope.ServiceProvider.GetService<IdentityServerDbContext>();
                context.Database.Migrate();

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var idnetityUser = userManager.FindByNameAsync("leo").Result;
                if (idnetityUser == null)
                {
                    idnetityUser = new User
                    {
                        UserName = "leo",
                        Email = "leo@email.com"
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

        #endregion

    }
}
