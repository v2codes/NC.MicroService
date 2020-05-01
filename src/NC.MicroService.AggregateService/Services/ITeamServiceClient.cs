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
        Task<IList<Team>> GetTeams();
    }
}
