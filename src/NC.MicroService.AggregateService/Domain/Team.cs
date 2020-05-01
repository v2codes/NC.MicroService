using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.AggregateService.Domain
{
    /// <summary>
    /// 团队
    /// </summary>
    public class Team
    {
        /// <summary>
        /// 团队ID
        /// </summary>
        public Guid Id { set; get; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public string TeamName { set; get; }

        /// <summary>
        /// 团队成员列表
        /// </summary>
        public IList<Member> Members { set; get; }
    }
}
