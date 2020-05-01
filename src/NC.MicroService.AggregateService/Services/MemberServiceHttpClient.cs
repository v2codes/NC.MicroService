using NC.MicroService.AggregateService.Domain;
using NC.MicroService.Infrastructure.Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.AggregateService.Services
{
    /// <summary>
    /// 成员服务调用实现
    /// </summary>
    public class MemberServiceHttpClient : IMemberServiceClient
    {
        // 协议 
        private readonly string _serviceScheme = "https";

        // 服务名称
        private readonly string _serviceName = "MemberService";

        // 资源路径
        private readonly string _requirePath = "/Members";

        // Consul HttpClient 请求
        private readonly ConsulHttpClient _consulHttpClient;

        public MemberServiceHttpClient(ConsulHttpClient consulHttpClient)
        {
            this._consulHttpClient = consulHttpClient;
        }

        /// <summary>
        /// 根据团队id查询团队成员
        /// </summary>
        /// <param name="teamId">团队ID</param>
        /// <returns></returns>
        public async Task<IList<Member>> GetMembersByTeamId(Guid teamId)
        {
            //// 1. 设置参数链接
            //string url = $"{_requirePath}?teamId={teamId}";

            // 2. 请求服务
            var members = await _consulHttpClient.GetAsync<List<Member>>(_serviceScheme, _serviceName, _requirePath);

            return members;
        }
    }
}
