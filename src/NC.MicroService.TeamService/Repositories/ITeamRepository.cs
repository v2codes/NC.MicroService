using NC.MicroService.TeamService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.TeamService.Repositories
{
    /// <summary>
    /// 团队模型仓储接口
    /// </summary>
    public interface ITeamRepository
    {
        IEnumerable<Team> GetTeams();
        Team GetTeamById(Guid id);
        void CreateTeam(Team team);
        void UpdateTeam(Team team);
        void DeleteTeam(Team team);
        bool TeamExists(Guid id);
    }
}
