using NC.MicroService.EntityFrameworkCore.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.VideoService.Domain
{
    /// <summary>
    /// 视频领域模型
    /// </summary>
    public class Video: EntityBase
    {
        /// <summary>
        /// 视频url
        /// </summary>
        public string VideoUrl { set; get; }

        /// <summary>
        /// 成员ID
        /// </summary>
        public Guid MemberId { set; get; }
    }
}
