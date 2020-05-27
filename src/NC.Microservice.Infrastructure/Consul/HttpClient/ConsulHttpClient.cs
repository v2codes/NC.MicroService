using NC.MicroService.Infrastructure.Culster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NC.MicroService.Infrastructure.Consul
{
    /// <summary>
    /// Consul HTTPClient扩展
    /// </summary>
    public class ConsulHttpClient
    {
        // Consul服务发现
        private readonly IServiceDiscovery _serviceDiscovery;

        // 负载均衡
        private readonly ILoadBalance _loadBalance;

        private readonly IHttpClientFactory _httpClientFactory;

        public ConsulHttpClient(IServiceDiscovery serviceDiscovery,
                                ILoadBalance loadBalance,
                                IHttpClientFactory httpClientFactory)
        {
            this._serviceDiscovery = serviceDiscovery;
            this._loadBalance = loadBalance;
            this._httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 服务请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceScheme">服务名称:(http/https)</param>
        /// <param name="serviceName">服务名称</param>
        /// <param name="requirePath">服务路径</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string serviceScheme, string serviceName, string requirePath)
        {
            // 1. 获取服务
            var serviceList = await _serviceDiscovery.Discovery(serviceName);

            if (serviceList == null || serviceList.Count == 0)
            {
                throw new Exception($"没有找到 {serviceName} 的可用服务。");
            }

            // 2. 负载均衡服务
            var service = _loadBalance.Select(serviceList);

            // 3. 请求服务
            var httpClient = _httpClientFactory.CreateClient("ConsulHttpClient");
            //var response = await httpClient.GetAsync($"{service.Url}{requirePath}");//TODO：Ocelot结合Consul会出错，注意Scheme.
            var response = await httpClient.GetAsync($"{serviceScheme}://{service.Url}{requirePath}");

            // 3.1 响应结果，json转换为对象
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
            else
            {
                throw new Exception($"{serviceName}服务调用异常");
            }
        }

        /// <summary>
        /// Post方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceScheme">服务名称:(http/https)</param>
        /// <param name="serviceName">服务名称</param>
        /// <param name="requirePath">服务路径</param>
        /// <param name="postData">POST数据</param>
        /// <returns></returns>
        public async Task<T> Post<T>(string serviceScheme, string serviceName, string requirePath, T postData = default(T))
        {
            // 1. 获取服务
            var serviceList = await _serviceDiscovery.Discovery(serviceName);
            if (serviceList == null || serviceList.Count == 0)
            {
                throw new Exception($"没有找到 {serviceName} 的可用服务。");
            }

            // 2. 负载均衡服务
            var service = _loadBalance.Select(serviceList);

            // 3. 建立请求
            Console.WriteLine($"请求路径：{serviceScheme} +'://'+{service.Url} + {requirePath}");
            var httpClient = _httpClientFactory.CreateClient("ConsulHttpClient");

            // 3.1 序列化对象为json数据
            var httpContext = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{serviceScheme}://{service.Url}{requirePath}", httpContext);

            // 3.2 反序列化响应数据为对象
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                // 3.3 进行自定义异常处理，这个地方进行了降级处理
                throw new Exception($"{serviceName}服务调用错误:{response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
