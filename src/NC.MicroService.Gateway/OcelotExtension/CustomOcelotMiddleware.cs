using Ocelot.Logging;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.Gateway.OcelotExtension
{
    /// <summary>
    /// 自定义 Ocelot 中间件
    /// </summary>
    public class CustomOcelotMiddleware : OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;

        public CustomOcelotMiddleware(OcelotRequestDelegate next, IOcelotLoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<CustomOcelotMiddleware>())
        {
        }

        public async Task Invoke(DownstreamContext context)
        {
            if (!context.IsError&&context.HttpContext.Request.Method.ToUpper()!="OPTIONS")
            {
                Console.WriteLine("======》 自定义中间件");
                Console.WriteLine("======》 自定义业务逻辑处理");
                // 1、处理统一结果
                // resutList resultMap 
                // 2、统一日志记录
                // 3、做链路监控
                // 4、性能监控
                // 5、流量统计
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
