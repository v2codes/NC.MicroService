using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NC.MicroService.TeamService.Domain;
using NC.MicroService.TeamService.EntityFrameworkCore;
using NC.MicroService.TeamService.Services;

namespace NC.MicroService.TeamService.Controllers
{
    [Route("Teams")]
    //[Authorize] // 1. 保护起来 ==> 由网关统一处理授权
    //[Authorize(Roles ="admin")] // 增加角色控制
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private ITeamService _teamService;
        private readonly IConfiguration _configuration;
        private readonly CoreContext _dbContext;

        public TeamsController(ITeamService teamService,
                               IConfiguration configuration, // 演示查看配置中心是否正常
                               CoreContext dbContext) // 演示动态数据库链接设置
        {
            _teamService = teamService;
            this._configuration = configuration;
            this._dbContext = dbContext;
        }

        /// <summary>
        /// 获取所有团队信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Team>> GetTeams()
        {
            Console.WriteLine("查询所有团队信息");

            #region 配置中心测试相关
            //// 1. 配置获取
            //Console.WriteLine($"配置中心配置项：Leo-Test={_configuration["Leo-Test"]}");
            //// 2. 动态设置数据连接
            //_dbContext.Database.GetDbConnection().ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            //// 3. 使用场景，缓存配置开关、服务降级
            //var useCache = _configuration["UseCache"];
            //if (useCache == "true")
            //{
            //    // 使用缓存，dosomething...
            //    return new List<Team>();
            //}
            //else
            //{
            //    // 不使用缓存，dosomething...
            //    return _teamService.QueryAll();
            //}
            #endregion

            // Thread.Sleep(10000000);
            // 1、演示宕机
            return _teamService.QueryAll();
        }

        /// <summary>
        /// 根据ID获取团队信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/Teams/{id}")]
        public ActionResult<Team> GetTeam([FromRoute]Guid id)
        {
            var team = _teamService.Find(id);

            if (team == null)
            {
                return NotFound();
            }
            return team;
        }

       /// <summary>
       /// 更新团队
       /// </summary>
       /// <param name="id"></param>
       /// <param name="team"></param>
       /// <returns></returns>
        [HttpPut("/Teams/{id}")]
        public IActionResult PutTeam([FromRoute]Guid id, [FromBody]Team team)
        {
            if (id != team.Id)
            {
                return BadRequest();
            }

            try
            {
                _teamService.Update(team);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_teamService.Exists(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("更新成功");
        }

        /// <summary>
        /// 创建团队
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            await _teamService.InsertAsync(team);

            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        /// <summary>
        /// 删除团队
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/Teams/{id}")]
        public ActionResult<Team> DeleteTeam([FromRoute]Guid id)
        {
            var team = _teamService.Find(id);
            if (team == null)
            {
                return NotFound();
            }

            _teamService.Delete(team);
            return team;
        }
    }
}
