using NC.Microservice.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NC.Microservice.Infrastructure.Consul
{
    /// <summary>
    /// 服务发现抽象接口
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        Task<IList<ServiceInfo>> Discovery(string serviceName);
    }
}
