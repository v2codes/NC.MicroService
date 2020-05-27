using NC.MicroService.AggregateService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.AggregateService.Services
{
    /// <summary>
    /// 成员服务调用接口
    /// </summary>
    public interface IMemberServiceClient
    {
        /// <summary>
        /// 根据团队id查询团队成员
        /// </summary>
        /// <returns></returns>
        Task<IList<Member>> GetMembersByTeamId(Guid teamId);

        /// <summary>
        /// 添加成员信息
        /// </summary>
        /// <param name="member"></param>
        Task InsertAsync(Member member);
    }
}
