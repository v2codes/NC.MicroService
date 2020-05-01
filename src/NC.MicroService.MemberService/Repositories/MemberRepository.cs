using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.MemberService.Domain;
using NC.MicroService.MemberService.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.MemberService.Repositories
{
    /// <summary>
    /// 成员仓储实现
    /// </summary>
    public class MemberRepository : RepositoryBase<Member, Guid>, IMemberRepository
    {
        public MemberRepository(CoreContext dbContext)
          : base(dbContext)
        {
        }
    }
}
