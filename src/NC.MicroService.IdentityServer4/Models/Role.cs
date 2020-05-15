using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.IdentityServer4.Models
{
    /// <summary>
    /// 自定义角色
    /// </summary>
    public class Role : IdentityRole<Guid>
    {
    }
}
