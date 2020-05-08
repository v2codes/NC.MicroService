using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NC.MicroService.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC.MicroService.Infrastructure.Consul
{
    /// <summary>
    /// 添加Consul中间件扩展 
    /// </summary>
    public static class ApplicationBuilderConsulExtensions
    {
        /// <summary>
        /// 添加 Consul 服务注册中间件到 IApplicationBuilder，用于将当前服务注册到 Consul 中
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsulRegistry(this IApplicationBuilder app)
        {
            // 1. 从IOC容器中获取 Consul 服务注册配置
            var serviceRegistryOption = app.ApplicationServices.GetRequiredService<IOptions<ServiceRegistryOptions>>().Value;

            // 2. 获取应用程序生命周期
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            // 2.1 获取服务注册实例
            var serviceRegistry = app.ApplicationServices.GetRequiredService<IServiceRegistry>();

            // 3. 动态获取服务地址
            var address = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
            var uri = new Uri(address);

            // 4. 注册服务
            serviceRegistryOption.Id = Guid.NewGuid().ToString();
            //serviceRegistryOption.Address = $"{uri.Scheme}://{uri.Host}"; //TODO：Ocelot结合Consul会出错，注意Scheme.
            serviceRegistryOption.Address = $"{uri.Host}";
            serviceRegistryOption.Port = uri.Port;
            serviceRegistryOption.HealthCheckAddress = $"{uri.Scheme}://{uri.Host}:{uri.Port}{serviceRegistryOption.HealthCheckAddress}";
            serviceRegistry.Register(serviceRegistryOption);

            // 5、服务器关闭时注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                serviceRegistry.Deregister(serviceRegistryOption);
            });

            return app;
        }
    }
}
