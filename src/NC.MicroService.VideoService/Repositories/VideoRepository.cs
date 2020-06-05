using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.VideoService.Domain;
using NC.MicroService.VideoService.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.VideoService.Repositories
{
    /// <summary>
    /// 视频模型仓储实现
    /// </summary>
    public class VideoRepository : RepositoryBase<Video, Guid>, IVideoRepository
    {
        public VideoRepository(CoreContext dbContext)
                : base(dbContext)
        {
        }
    }
}
