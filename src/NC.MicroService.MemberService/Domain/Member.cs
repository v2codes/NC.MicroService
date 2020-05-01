using NC.MicroService.EntityFrameworkCore.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.MemberService.Domain
{
    /// <summary>
    /// 成员领域模型
    /// </summary>
    public class Member : EntityBase
    {
        /// <summary>
        /// 成员姓名
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 成员昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>                                                      
        /// 所属团队ID
        /// </summary>
        public Guid? TeamId { get; set; }
    }
}
