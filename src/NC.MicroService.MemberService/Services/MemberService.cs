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
    }
}
