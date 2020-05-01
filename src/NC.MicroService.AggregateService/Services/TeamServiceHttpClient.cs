using NC.MicroService.AggregateService.Domain;
using NC.MicroService.Infrastructure.Consul;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NC.MicroService.AggregateService.Services
{
    /// <summary>
    /// 团队服务调用实现
    /// </summary>
    public class TeamServiceHttpClient : ITeamServiceClient
    {
        // 协议 
        private readonly string _serviceScheme = "https";

        // 服务名称
        private readonly string _serviceName = "TeamService";

        // 资源路径
        private readonly string _requirePath = "/Teams";

        // Consul HttpClient 请求
        private readonly ConsulHttpClient _consulHttpClient;

        //public TeamServiceHttpClient(ConsulHttpClient consulHttpClient)
        //{
        //    this._consulHttpClient = consulHttpClient;
        //}

        private readonly IHttpClientFactory _httpClientFactory;
        public TeamServiceHttpClient(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 获取所有团队
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Team>> GetTeams()
        {
            // 查询所有团队
            var teams = await _consulHttpClient.GetAsync<List<Team>>(_serviceScheme, _serviceName, _requirePath);
            return teams;

            // 测试熔断降级，请启用此处代码！！！！！！！
            //for (int i = 0; i < 100; i++)
            //{
            //    try
            //    {
            //        Thread.Sleep(1000);
            //        HttpClient httpClient = _httpClientFactory.CreateClient("ncmicroservices");
            //        HttpResponseMessage response = await httpClient.GetAsync("https://localhost:5001" + _requirePath);

            //        // 3.1json转换成对象
            //        IList<Team> teams = null;
            //        if (response.StatusCode == HttpStatusCode.OK)
            //        {
            //            string json = await response.Content.ReadAsStringAsync();

            //            teams = JsonConvert.DeserializeObject<List<Team>>(json);
            //        }
            //        else
            //        {
            //            Console.WriteLine($"降级处理：{await response.Content.ReadAsStringAsync()}");
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine($"异常捕获：{e.Message}");
            //    }
            //}
            //return null;
        }
    }
}
