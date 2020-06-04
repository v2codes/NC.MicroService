using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.TeamService.Domain;
using NC.MicroService.TeamService.Repositories;
using Servicecomb.Saga.Omega.Abstractions.Transaction;
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

        /// <summary>
        /// saga事务参与者 Compensable撤销业务 逻辑 --> 子事务
        /// </summary>
        /// <param name="team"></param>
        [Compensable(nameof(CancelTeamInsert))]
        public override async Task<int> InsertAsync(Team entity)
        {
            var result = await _repository.InsertAsync(entity);
            return result;
        }

        void CancelTeamInsert(Team entity)
        {
            base.Delete(entity);
        }
    }
}
