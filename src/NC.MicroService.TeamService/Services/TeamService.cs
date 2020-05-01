using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.TeamService.Domain;
using NC.MicroService.TeamService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.TeamService.Services
{
    /// <summary>
    /// 团队仓储服务实现
    /// </summary>
    public class TeamService : ServiceBase<Team, Guid>, ITeamService
    {
        public TeamService(ITeamRepository teamRepository)
            : base(teamRepository)
        {
        }
    }
}
