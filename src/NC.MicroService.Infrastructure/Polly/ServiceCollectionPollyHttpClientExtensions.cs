using Microsoft.Extensions.DependencyInjection;
using Polly;
using NC.MicroService.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NC.MicroService.Infrastructure.Polly
{
    /// <summary>
    /// 注册 Polly HttpClient服务注入IOC容器扩展
    /// 超时、重试、熔断、降级策略
    /// </summary>
    public static class ServiceCollectionPollyHttpClientExtensions
    {
        /// <summary>
        /// 注册 Polly HttpClient服务注入IOC容器扩展
        /// </summary>
        /// <param name="services">ioc容器</param>
        /// <param name="httpClientName">HttpClient 名称(针对不同的服务进行熔断，降级)</param>
        /// <param name="action">熔断降级配置</param>
        /// <param name="TResult">降级处理错误的结果</param>
        public static IServiceCollection AddPollyHttpClient(this IServiceCollection services, string httpClientName, Action<PollyHttpClientOptions> action)
        {
            // 1. 创建选项配置类
            var options = new PollyHttpClientOptions();
            action(options);

            // 2. 配置 HttpClient 各种策略
            services.AddHttpClient(httpClientName)

            // 1）降级策略
            .AddPolicyHandler(Policy<HttpResponseMessage>.HandleInner<Exception>().FallbackAsync(options.httpResponseMessage, async b =>
            {
                // 1、降级打印异常
                Console.WriteLine($"服务{httpClientName}开始降级,异常消息：{b.Exception.Message}");
                // 2、降级后的数据
                Console.WriteLine($"服务{httpClientName}降级内容响应：{await options.httpResponseMessage.Content.ReadAsStringAsync()}");
                await Task.CompletedTask;
            }))

            // 2) 熔断策略
            .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<Exception>().CircuitBreakerAsync(options.CircuitBreakerOpenFallCount, TimeSpan.FromSeconds(options.CircuitBreakerDownTime), (ex, ts) =>
            {
                Console.WriteLine($"服务{httpClientName}断路器开启，异常消息：{ex.Exception.Message}");
                Console.WriteLine($"服务{httpClientName}断路器开启时间：{ts.TotalSeconds}s");
            }, () =>
            {
                Console.WriteLine($"服务{httpClientName}断路器关闭");
            }, () =>
            {
                Console.WriteLine($"服务{httpClientName}断路器半开启(时间控制，自动开关)");
            }))

            // 2) 重试策略
             .AddPolicyHandler(Policy<HttpResponseMessage>
              .Handle<Exception>()
              .RetryAsync(options.RetryCount)
            )

            // 1.4 超时策略
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(options.TimeoutTime)));



            return services;
        }
    }
}
