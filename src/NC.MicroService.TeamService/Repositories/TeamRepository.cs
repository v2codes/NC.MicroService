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

        public void CreateTeam(Team team)
        {
            Insert(team);
        }


        public Team GetTeamById(Guid id)
        {
            return this.Find(id);
        }

        public void DeleteTeam(Team team)
        {
            this.DeleteTeam(team.Id);
        }

        public void DeleteTeam(Guid id)
        {
            this.Delete(id);
        }

        public void UpdateTeam(Team team)
        {
            this.Update(team);
        }

        public bool TeamExists(Guid id)
        {
            return this.Exist(p => p.Id == id);
        }

        public IEnumerable<Team> GetTeams()
        {
            return this.QueryAll().ToList();
        }
    }
}
