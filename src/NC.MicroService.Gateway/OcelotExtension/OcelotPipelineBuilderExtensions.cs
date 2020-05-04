using Ocelot.Middleware.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.Gateway.OcelotExtension
{

    /// <summary>
    /// 注册ocelot中间件
    /// </summary>
    public static class OcelotPipelineBuilderExtensions
    {
        public static IOcelotPipelineBuilder UseCustomResponseMiddleware(this IOcelotPipelineBuilder builder)
        {
            return builder.UseMiddleware<CustomOcelotMiddleware>();
        }
    }
}
