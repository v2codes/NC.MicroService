using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.TeamService.Domain;
using NC.MicroService.TeamService.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.TeamService.Repositories
{
    /// <summary>
    /// 团队模型仓储实现
    /// </summary>
    public class TeamRepository : RepositoryBase<Team, Guid>, ITeamRepository
    {
        public TeamRepository(CoreContext dbContext)
                : base(dbContext)
        {
        }
    }
}
