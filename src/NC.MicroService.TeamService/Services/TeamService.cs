using NC.MicroService.TeamService.Domain;
using NC.MicroService.TeamService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.TeamService.Services
{
    public class TeamService : ITeamService
    {
        private ITeamRepository _teamRepository;
        public TeamService(ITeamRepository teamRepository)
        {
            this._teamRepository = teamRepository;
        }

        public void CreateTeam(Team team)
        {
            _teamRepository.CreateTeam(team);
        }

        public void DeleteTeam(Team team)
        {
            _teamRepository.DeleteTeam(team);
        }

        public Team GetTeamById(Guid id)
        {
            return _teamRepository.GetTeamById(id);
        }

        public IEnumerable<Team> GetTeams()
        {
            return _teamRepository.GetTeams();
        }

        public bool TeamExists(Guid id)
        {
            return _teamRepository.TeamExists(id);
        }

        public void UpdateTeam(Team team)
        {
            _teamRepository.UpdateTeam(team);
        }
    }
}
