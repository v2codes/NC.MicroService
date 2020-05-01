using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.MemberService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.MemberService.Repositories
{
    /// <summary>
    /// 成员仓储接口
    /// </summary>
    public interface IMemberRepository : IRepository<Member, Guid>
    {
    }
}
