using NC.MicroService.EntityFrameworkCore.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.TeamService.Domain
{
    /// <summary>
    /// 团队
    /// 领域模型
    /// </summary>
    public class Team : EntityBase
    {
        /// <summary>
        /// 团队名称
        /// </summary>
        public string TeamName { set; get; }
    }
}
