using Consul;
using Microsoft.Extensions.Configuration;
using NC.Microservice.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NC.Microservice.Infrastructure.Consul
{
    /// <summary>
    /// Consul 服务发现
    /// </summary>
    public class ConsulServiceDiscovery : IServiceDiscovery
    {
        private readonly IConfiguration _configuration;
        public ConsulServiceDiscovery(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<IList<ServiceInfo>> Discovery(string serviceName)
        {
            var serviceDiscoveryOption = _configuration.GetSection("ConsulDiscovery").Get<ServiceDiscoveryOptions>();

            // 1. 创建 Consul 客户端连接
            var consulClient = new ConsulClient(config =>
            {
                // 1.1 配置 Consul 服务端地址
                config.Address = new Uri(serviceDiscoveryOption.RegistryAddress);
            });

            // 2. Consul 查询服务，根据具体的服务名称查询
            var serviceList = await consulClient.Catalog.Service(serviceName);

            // 3. 将服务进行拼接
            var services = new List<ServiceInfo>();
            foreach (var item in serviceList.Response)
            {
                services.Add(new ServiceInfo { Url = $"{item.ServiceAddress}:{item.ServicePort}" });
            }
            return services;
        }
    }
}
