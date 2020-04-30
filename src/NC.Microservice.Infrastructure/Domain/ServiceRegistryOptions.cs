using System;
using System.Collections.Generic;
using System.Text;

namespace NC.MicroService.Infrastructure.Domain
{
    /// <summary>
    /// 服务注册配置选项
    /// </summary>
    public class ServiceRegistryOptions
    {
        public ServiceRegistryOptions()
        {
            this.Timeout = 10;
            this.DeregisterAfterServiceStop = 5;
            this.HealthCheckInterval = 10;
        }

        /// <summary>
        /// 服务ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 服务标签(版本)
        /// </summary>
        public string[] Tags { set; get; }

        /// <summary>
        /// 服务地址(可以选填 === 默认加载启动路径)
        /// </summary>
        public string Address { set; get; }

        /// <summary>
        /// 服务端口号(可以选填 === 默认加载启动路径端口)
        /// </summary>
        public int Port { set; get; }

        /// <summary>
        /// 服务注册地址
        /// </summary>
        public string RegistryAddress { get; set; }

        /// <summary>
        /// 服务健康检查地址
        /// </summary>
        public string HealthCheckAddress { get; set; }

        /// <summary>
        /// 健康检查超时时间（秒）
        /// 默认10秒
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 服务停止指定时间后执行注销（秒）
        /// 默认5秒
        /// </summary>
        public int DeregisterAfterServiceStop { get; set; }

        /// <summary>
        /// 健康检查间隔时间（秒）
        /// 默认10秒
        /// </summary>
        public int HealthCheckInterval { get; set; }
    }
}
