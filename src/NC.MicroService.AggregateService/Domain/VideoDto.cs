using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.AggregateService.Domain
{
    /// <summary>
    /// 视频模型
    /// </summary>
    public class VideoDto
    {
        /// <summary>
        /// 视频主键
        /// </summary>
        public Guid Id { set; get; }

        /// <summary>
        /// 视频url
        /// </summary>
        public string VideoUrl { set; get; }

        /// <summary>
        /// 成员Id
        /// </summary>
        public Guid MemberId { set; get; }
    }
}
