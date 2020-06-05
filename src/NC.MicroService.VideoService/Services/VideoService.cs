using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.VideoService.Domain;
using NC.MicroService.VideoService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.VideoService.Services
{
    /// <summary>
    /// 团队仓储服务实现
    /// </summary>
    public class VideoService : ServiceBase<Video, Guid>, IVideoService
    {
        public VideoService(IVideoRepository videoRepository)
            : base(videoRepository)
        {

        }
    }
}
