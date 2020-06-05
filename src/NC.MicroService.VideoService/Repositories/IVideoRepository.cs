using NC.MicroService.EntityFrameworkCore.Repository;
using NC.MicroService.VideoService.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.VideoService.Repositories
{
    /// <summary>
    /// 团队模型仓储接口
    /// </summary>
    public interface IVideoRepository : IRepository<Video, Guid>
    {
    }
}
