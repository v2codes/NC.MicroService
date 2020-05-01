using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.AggregateService.Domain
{
    /// <summary>
    /// 成员
    /// </summary>
    public class Member
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        public Guid Id { set; get; }

        /// <summary>
        /// 成员姓名
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 成员昵称
        /// </summary>
        public string NickName { get; set; }
    }
}
