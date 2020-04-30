using Consul;
using NC.Microservice.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC.Microservice.Infrastructure.Consul
{
    /// <summary>
    /// Consul 服务注册
    /// </summary>
    public class ConsulServiceRegistry : IServiceRegistry
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serviceRegistryOption">服务注册配置信息</param>
        public void Register(ServiceRegistryOptions serviceRegistryOption)
        {
            // 1. 创建 Consul 客户端连接
            var consulClient = new ConsulClient(config =>
            {
                // 1.1 配置 Consul 服务端地址
                config.Address = new Uri(serviceRegistryOption.Address);
            });

            // 2. 获取服务内部地址
            // TODO...

            // 3. 创建 Consul 服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = serviceRegistryOption.Id,
                Name = serviceRegistryOption.Name,
                Address = serviceRegistryOption.Address,
                Port = serviceRegistryOption.Port,
                Tags = serviceRegistryOption.Tags,
                Check = new AgentServiceCheck()
                {
                    // 3.1 Consul 健康检查超时时间
                    Timeout = TimeSpan.FromSeconds(serviceRegistryOption.Timeout),
                    // 3.2 服务停止指定时间后执行注销
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(serviceRegistryOption.DeregisterAfterServiceStop),
                    // 3.3 Consul 健康检查地址
                    HTTP = serviceRegistryOption.HealthCheckAddress,
                    // 3.4 Consul 健康检查间隔时间
                    Interval = TimeSpan.FromSeconds(serviceRegistryOption.HealthCheckInterval),
                }
            };

            // 4. 注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();

            // 5. 关闭连接
            consulClient.Dispose();
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="serviceRegistryOption">服务注册配置信息</param>
        public void Deregister(ServiceRegistryOptions serviceRegistryOption)
        {
            // 1. 创建 Consul 客户端连接
            var consulClient = new ConsulClient(config =>
            {
                // 1.1 配置 Consul 服务端地址
                config.Address = new Uri(serviceRegistryOption.Address);
            });

            // 2. 注销服务
            consulClient.Agent.ServiceDeregister(serviceRegistryOption.Id);

            // 3. 关闭连接
            consulClient.Dispose();
        }
    }
}
