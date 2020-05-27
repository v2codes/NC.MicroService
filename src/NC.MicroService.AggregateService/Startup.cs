using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NC.MicroService.AggregateService.Services;
using NC.MicroService.Infrastructure.Consul;
using NC.MicroService.Infrastructure.Culster;
using NC.MicroService.Infrastructure.Polly;
using Polly;

namespace NC.MicroService.AggregateService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 1、自定义异常处理(用缓存处理)
            var fallbackResponse = new HttpResponseMessage
            {
                Content = new StringContent("系统正繁忙，请稍后重试"),// 内容，自定义内容
                StatusCode = HttpStatusCode.GatewayTimeout // 504
            };

            /*********** 测试Polly效果修改 TeamServiceHttpClient 中的 GetTeams 方法，并且确保 TeamService 不要启动 ***********/

            #region Polly启用：超时、重试、熔断、降级

            //services.AddHttpClient("ncmicroservices") // 使用命名是为了使用返回值为 IHttpClientBuilder 的构造函数，否则返回的是 IServiceCollection

            //        // 降级
            //        .AddPolicyHandler(Policy<HttpResponseMessage>
            //                // 方式1：默认内置异常处理
            //                .Handle<Exception>()
            //                .FallbackAsync(fallbackResponse, async res =>
            //                {
            //                    // 1、降级打印异常
            //                    Console.WriteLine($"服务开始降级,异常消息：{res.Exception.Message}");
            //                    // 2、降级后的数据
            //                    Console.WriteLine($"服务降级内容响应：{await fallbackResponse.Content.ReadAsStringAsync()}");
            //                    await Task.CompletedTask;
            //                }))
            //        // 熔断
            //        .AddPolicyHandler(Policy<HttpResponseMessage> // 异步策略：IAsyncPolicy<HttpResponseMessage> policy
            //                .Handle<Exception>() // 捕获异常
            //                                     // 开启断路器，熔断机制
            //                                     // 参数 3：出现指定次数异常，开启熔断
            //                                     // 参数 TimeSpan.FromSeconds(10)：熔断器开启时长
            //                                     // 参数 第1个委托回调函数：断路器开启回调通知
            //                                     // 参数 第2个委托回调函数：断路器重置回调通知
            //                                     // 参数 第2个委托回调函数：断路器半开启回调通知，
            //                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10), (ex, ts) =>
            //                {
            //                    Console.WriteLine($"服务断路器开启，异常消息：{ex.Exception.Message}");
            //                    Console.WriteLine($"服务断路器开启时间：{ts.TotalSeconds}s");
            //                }, () =>
            //                {
            //                    Console.WriteLine($"服务断路器重置");
            //                }, () =>
            //                {
            //                    Console.WriteLine($"服务断路器半开启(一会开，一会关)");
            //                }))
            //        // 重试
            //        .AddPolicyHandler(Policy<HttpResponseMessage>
            //                .Handle<Exception>()
            //                .RetryAsync(3))
            //        // 超时
            //        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(2)));

            #endregion

            // 1.2 封装之后的 Polly 启用
            services.AddPollyHttpClient("ncmicroservices", options =>
            {
                options.TimeoutTime = 60; // 1、超时时间
                options.RetryCount = 3;// 2、重试次数
                options.CircuitBreakerOpenFallCount = 2;// 3、熔断器开启(多少次失败开启)
                options.CircuitBreakerDownTime = 100;// 4、熔断器开启时间
                options.httpResponseMessage = fallbackResponse;// 5、降级处理
            })
            .AddConsulClient<ConsulHttpClient>(); // 1.3、HttpClient下consul封装(实现负载均衡请求)

            //// 1. 注入 Consul HttpClient，该注册扩展方法中包含了 Consul服务发现、负载均衡服务注册 --> 省却2、3步骤
            //services.AddHttpClient().AddConsulClient<ConsulHttpClient>();

            /*// 2. 注册
            services.AddConsulDiscovery();
            // 3. 注册负载均衡
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();*/

            // 4. 注册team服务
            services.AddSingleton<ITeamServiceClient, TeamServiceHttpClient>();

            // 5. 注册成员服务
            services.AddSingleton<IMemberServiceClient, MemberServiceHttpClient>();

            // 6. 注册 Consul服务注册
            services.AddConsulRegistry(Configuration);

            // 6. 校验AccessToken,从身份校验中心进行校验 --> 参加 TeamService

            //// 7. 注册Saga分布式事务
            //services.AddOmegaCore(options =>
            //{
            //    options.GrpcServerAddress = "LL2019:8080"; // 7.1 协调中心地址 alpha
            //    options.InstanceId = "AggregateService-ID"; // 7.2 服务实例ID -- 用于集群
            //    options.ServiceName = "AggregateService"; // 7.3 服务名称
            //});

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 1. Consul服务注册
            app.UseConsulRegistry();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
