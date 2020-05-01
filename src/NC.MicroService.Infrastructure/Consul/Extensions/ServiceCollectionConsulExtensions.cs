using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NC.MicroService.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC.MicroService.Infrastructure.Consul
{
    /// <summary>
    /// 注册 Consul注册和发现服务注入IOC容器扩展（加载配置） 
    /// </summary>
    public static class ServiceCollectionConsulExtensions
    {
        /// <summary>
        /// 注册 Consul注册服务到IOC容器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulRegistry(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. 加载 Consul 服务注册配置
            services.Configure<ServiceRegistryOptions>(configuration.GetSection("ConsulRegistry"));

            // 2. 注册 Consul服务注册
            services.AddSingleton<IServiceRegistry, ConsulServiceRegistry>();
            return services;
        }

        /// <summary>
        /// 注册 Consul发现服务到IOC容器
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulDiscovery(this IServiceCollection services)
        {
            // 1. 加载 Consul 服务发现配置
            //services.Configure<ServiceDiscoveryOption>(configuration.GetSection("ConsulDiscovery"));

            // 2. 注册 Consul服务发现
            services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
            return services;
        }
    }
}
