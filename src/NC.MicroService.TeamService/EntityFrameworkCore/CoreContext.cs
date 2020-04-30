using Microsoft.EntityFrameworkCore;
using NC.MicroService.TeamService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.TeamService.EntityFrameworkCore
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class CoreContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public CoreContext(DbContextOptions<CoreContext> options)
            : base(options)
        {

        }
    }
}
