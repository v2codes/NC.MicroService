using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.MemberService.Domain;
using NC.MicroService.MemberService.Repositories;
using Servicecomb.Saga.Omega.Abstractions.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.MemberService.Services
{
    /// <summary>
    /// 成员仓储服务实现
    /// </summary>
    public class MemberService : ServiceBase<Member, Guid>, IMemberService
    {
        public MemberService(IMemberRepository repository)
            : base(repository)
        {

        }

        /// <summary>
        /// saga事务参与者 Compensable撤销业务 逻辑 --> 子事务
        /// </summary>
        /// <param name="team"></param>
        [Compensable(nameof(CancelMemberInsert))]
        public override async Task<int> InsertAsync(Member entity)
        {
            var res = await _repository.InsertAsync(entity);
            return res;
        }

        void CancelMemberInsert(Member entity)
        {
            Console.WriteLine("Rollback...");
            base.Delete(entity);
        }
    }
}
