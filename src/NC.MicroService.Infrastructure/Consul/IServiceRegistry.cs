using NC.MicroService.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NC.MicroService.Infrastructure.Consul
{
    /// <summary>
    /// 服务注册抽象接口
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="option"></param>
        void Register(ServiceRegistryOptions serviceRegistryOption);
        /// <summary>
        /// 撤销服务
        /// </summary>
        /// <param name="option"></param>
        void Deregister(ServiceRegistryOptions serviceRegistryOption);
    }
}
