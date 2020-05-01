using Microsoft.EntityFrameworkCore;
using NC.MicroService.MemberService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.MemberService.EntityFrameworkCore
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class CoreContext : DbContext
    {
        public CoreContext(DbContextOptions<CoreContext> options)
            : base(options)
        {

        }
        public DbSet<Member> Members { get; set; }

    }
}
