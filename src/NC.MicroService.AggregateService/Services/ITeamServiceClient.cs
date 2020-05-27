using NC.MicroService.AggregateService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.AggregateService.Services
{
    /// <summary>
    /// 团队服务调用接口
    /// </summary>
    public interface ITeamServiceClient
    {
        /// <summary>
        /// 查询所有团队信息
        /// </summary>
        /// <returns></returns>
        Task<IList<Team>> GetTeams();

        /// <summary>
        /// 添加团队信息
        /// </summary>
        /// <param name="team"></param>
        Task InsertAsync(Team team);
    }
}
