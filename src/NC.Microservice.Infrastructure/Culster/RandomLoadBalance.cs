using NC.MicroService.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC.MicroService.Infrastructure.Culster
{
    /// <summary>
    /// 随机负载均衡
    /// 1.还可以实现加权轮训 // TODO
    /// </summary>
    public class RandomLoadBalance : AbstractLoadBalance
    {
        private readonly Random _random = new Random();
        public override ServiceInfo DoSelect(IList<ServiceInfo> services)
        {
            // 1. 获取随机数
            var index = _random.Next(services.Count);

            // 2. 选择一个服务
            return services[index];
        }
    }
}
