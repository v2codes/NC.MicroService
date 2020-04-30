using System;
using System.Collections.Generic;
using System.Text;

namespace NC.MicroService.Infrastructure.Domain
{
    /// <summary>
    /// 服务发现配置选项
    /// </summary>
    public class ServiceDiscoveryOptions
    {
        /// <summary>
        /// 服务注册地址
        /// </summary>
        public string RegistryAddress { get; set; }
    }
}
