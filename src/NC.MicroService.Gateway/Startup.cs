using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NC.MicroService.Gateway.IdentityServer;
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
            #region 集成IdentityServer4，校验AccessToken,从身份校验中心进行校验
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //  // 服务中开启授权认证，只需要在接口上添加 [Authorize] 特性即可
            //  // 而在网关中并没有类似于Controller的东西可以给你添加授权特性，那么，网关如何知道已经开启了身份认证呢？
            //  // 在这个方法中，第一个参数 authenticationScheme，就可以告诉网关哪个服务需要身份授权校验，，TODO... 
            //  .AddIdentityServerAuthentication("OcelotKey", options =>
            //  {
            //      options.Authority = "https://192.168.2.102:5005"; // 1. 授权中心地址
            //      options.ApiName = "OcelotService"; // 2. api名称(项目具体名称)，注意数据库中的 apiresource表对应
            //      options.RequireHttpsMetadata = true; // 3. https元数据，不需要
            //      options.JwtBackChannelHandler = GetHandler(); // 4. 自定义 HttpClientHandler 
            //  });
            #endregion

            #region 配置项方式集成 IdentityServer4
            // 绑定 IdentityServer 配置信息
            var identityServerOptions = new IdentityServerOptions();
            Configuration.Bind("IdentityServerOptions", identityServerOptions);

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
              // 服务中开启授权认证，只需要在接口上添加 [Authorize] 特性即可
              // 而在网关中并没有类似于Controller的东西可以给你添加授权特性，那么，网关如何知道已经开启了身份认证呢？
              // 在这个方法中，第一个参数 authenticationScheme，就可以告诉网关哪个服务需要身份授权校验，，TODO... 
              .AddIdentityServerAuthentication(identityServerOptions.IdentityScheme, options =>
              {
                  options.Authority = identityServerOptions.AuthorityAddress; // 1. 授权中心地址
                  options.ApiName = identityServerOptions.ResourceName; // 2. api名称(项目具体名称)，注意数据库中的 apiresource表对应
                  options.RequireHttpsMetadata = true; // 3. https元数据，不需要
                  options.JwtBackChannelHandler = GetHandler(); // 4. 自定义 HttpClientHandler 
              });
            #endregion


            // 1. 注册网关 Ocelot 到IOC容器
            //services.AddOcelot(); // 注册 Ocelot

            //services.AddOcelot().AddConsul(); // 注册 Ocelot，结合 Consul，实现动态路由

            services.AddOcelot(Configuration).AddConsul().AddPolly(); // 注册 Ocelot，结合 Consul 实现动态路由，结合 Polly 实现熔断/降级。
        }

        /// <summary>
        /// 自定义 HttpClientHandler ，避开证书验证问题：IdentityServer4 HTTPS IDX20804 IDX20803
        /// </summary>
        /// <returns></returns>
        private static HttpClientHandler GetHandler()
        {
            var handler = new HttpClientHandler();
            //handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                return true;
            };
            return handler;
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

            // 2、使用网关
            app.UseOcelot((build, config) =>
            {
                build.BuildCustomeOcelotPipeline(config); // 2.1 自定义ocelot中间件注册完成
            }).Wait();


            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
