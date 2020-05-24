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

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public ActionResult<Team> GetTeam(Guid id)
        {
            var team = _teamService.Find(id);

            if (team == null)
            {
                return NotFound();
            }
            return team;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public IActionResult PutTeam(Guid id, Team team)
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

            return NoContent();
        }

        // POST: api/Teams
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public ActionResult<Team> PostTeam(Team team)
        {
            _teamService.Insert(team);

            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public ActionResult<Team> DeleteTeam(Guid id)
        {
            var team = _teamService.Find(id);
            if (team == null)
            {
                return NotFound();
            }

            _teamService.Delete(id);
            return team;
        }
    }
}
