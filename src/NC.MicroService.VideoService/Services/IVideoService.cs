using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.VideoService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.VideoService.Services
{
    /// <summary>
    /// 视频仓储服务接口
    /// </summary>
    public interface IVideoService : IService<Video, Guid>
    {
    }
}
