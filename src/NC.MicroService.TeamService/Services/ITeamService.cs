using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.TeamService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.TeamService.Services
{
    /// <summary>
    /// 团队仓储服务接口
    /// </summary>
    public interface ITeamService : IService<Team, Guid>
    {
    }
}
