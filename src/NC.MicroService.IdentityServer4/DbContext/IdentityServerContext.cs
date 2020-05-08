using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.IdentityServer4.DbContext
{
    /// <summary>
    /// IdentityServer4 数据库上下文
    /// </summary>
    public class IdentityServerContext:IdentityDbContext<IdentityUser>
    {
        public IdentityServerContext(DbContextOptions<IdentityServerContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
