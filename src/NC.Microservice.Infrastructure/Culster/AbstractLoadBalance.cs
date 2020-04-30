using NC.Microservice.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC.Microservice.Infrastructure.Culster
{
    /// <summary>
    /// 负载均衡抽象实现
    /// </summary>
    public abstract class AbstractLoadBalance : ILoadBalance
    {
        public ServiceInfo Select(IList<ServiceInfo> services)
        {
            if (services == null || services.Count == 0)
                return null;
            if (services.Count == 1)
                return services[0];
            return DoSelect(services);
        }

        /// <summary>
        /// 负载均衡策略抽象方法
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public abstract ServiceInfo DoSelect(IList<ServiceInfo> services);
    }
}
