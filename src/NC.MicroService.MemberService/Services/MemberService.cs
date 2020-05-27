using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.MemberService.Domain;
using NC.MicroService.MemberService.Repositories;
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
        /// saga事务参与者 Compensable撤销业务 逻辑
        /// </summary>
        /// <param name="team"></param>
        //[Compensable(nameof(Delete))]
        public override async Task<int> InsertAsync(Member entity)
        {
            return await _repository.InsertAsync(entity);
        }
    }
}
