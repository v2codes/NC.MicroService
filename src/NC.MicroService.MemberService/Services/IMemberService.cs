using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.MemberService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.MemberService.Services
{
    /// <summary>
    /// 成员仓储服务接口
    /// </summary>
    public interface IMemberService : IService<Member, Guid>
    {
    }
}
