using NC.Microservice.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC.Microservice.Infrastructure.Culster
{
    /// <summary>
    /// 服务负载均衡抽象接口
    /// </summary>
    public interface ILoadBalance
    {
        /// <summary>
        /// 服务选择
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        ServiceInfo Select(IList<ServiceInfo> services);
    }
}
