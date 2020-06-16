using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NC.MicroService.VideoService.Domain;
using NC.MicroService.VideoService.Services;

namespace NC.MicroService.VideoService.Controllers
{
    /// <summary>
    /// 团队微服务api
    /// </summary>
    [Route("Video")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public HomeController(IVideoService videoService)
        {
            this._videoService = videoService;
        }

        // GET: api/Videos
        [HttpGet]
        public ActionResult<IEnumerable<Video>> GetVideos()
        {
            return _videoService.QueryAll();
        }

        // GET: api/Videos/5
        [HttpGet("/Videos/{id}")]
        public ActionResult<Video> GetVideo([FromRoute] Guid id)
        {
            var Video = _videoService.Find(id);
            if (Video == null)
            {
                return NotFound();
            }
            return Video;
        }

        // PUT: api/Videos/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("/Videos/{id}")]
        public IActionResult PutVideo([FromRoute] Guid id, [FromBody] Video Video)
        {
            if (id != Video.Id)
            {
                return BadRequest();
            }

            try
            {
                _videoService.Update(Video);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_videoService.Exists(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// 视频添加
        /// </summary>
        /// <param name="Video"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe("videoCreateEvent")] // 事件消息名 
        public ActionResult<Video> PostVideo([FromBody] Video Video)
        {
            // 1、阻塞30
            // Thread.Sleep(30000);// 不会影响客户端响应速度，video数据保存会延迟
            // throw new Exception("出现异常");
            Console.WriteLine($"接受到视频事件消息");
            _videoService.Insert(Video);
            return CreatedAtAction("GetVideo", new { id = Video.Id }, Video);
        }

        ///// <summary>
        ///// 视频添加（基于CAP的异步添加）
        ///// </summary>
        ///// <param name="Video"></param>
        ///// <returns></returns>
        ///// video.event video.event  video.event.1
        ///// video.event video.1 video.1
        ///// *  一对多匹配 --> 描述不对！！！
        ///// # 一对一匹配 --> 描述不对！！！
        ///// ==> “#”匹配一个或多个词，符号“*”匹配一个词。因此“audit.#”能够匹配到“audit.irs.corporate”，但是“audit.*” 只会匹配到“audit.irs”。
        //[NonAction]
        //[CapSubscribe("video.*")]
        //public ActionResult<Video> PostVideo([FromBody] Video Video)
        //{
        //    // 1、阻塞30
        //    // Thread.Sleep(30000);
        //    // throw new Exception("出现异常");
        //    Console.WriteLine($"接受到视频事件消息");
        //    _videoService.Insert(Video);
        //    return CreatedAtAction("GetVideo", new { id = Video.Id }, Video);
        //}

        // DELETE: api/Videos/5
        [HttpDelete("/Videos/{id}")]
        public ActionResult<Video> DeleteVideo([FromRoute]Guid id)
        {
            var Video = _videoService.Find(id);
            if (Video == null)
            {
                return NotFound();
            }

            _videoService.Delete(Video);
            return Video;
        }
    }
}
