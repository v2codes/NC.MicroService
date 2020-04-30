using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NC.MicroService.Infrastructure.Culster;
using NC.MicroService.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC.MicroService.Infrastructure.Consul
{
    /// <summary>
    /// 添加 HttpClientFactory conusl下的扩展 
    /// </summary>
    public static class ServiceCollectionConsulHttpClientExtensions
    {

        public static IServiceCollection AddHttpClientConsul<ConsulHttpClient>(this IServiceCollection services) where ConsulHttpClient : class
        {
            // 1、注册consul
            services.AddConsulDiscovery();

            // 2、注册服务负载均衡
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();

            // 3、注册httpclient
            services.AddSingleton<ConsulHttpClient>();

            return services;
        }
    }
}
