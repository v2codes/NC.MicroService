using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NC.MicroService.AggregateService.Domain;
using NC.MicroService.AggregateService.Services;
using Servicecomb.Saga.Omega.Abstractions.Transaction;

namespace NC.MicroService.AggregateService.Controllers
{
    [Route("api/Teams")]
    [ApiController]
    public class AggregateController : ControllerBase
    {

        private readonly ITeamServiceClient _teamServiceClient;
        private readonly IMemberServiceClient _memberServiceClient;

        private readonly IConfiguration _configuration;

        public AggregateController(ITeamServiceClient teamServiceClient,
                                   IMemberServiceClient memberServicesClient,
                                   IConfiguration configuration) // 演示查看配置中心是否正常 
        {
            this._teamServiceClient = teamServiceClient;
            this._memberServiceClient = memberServicesClient;
            this._configuration = configuration;
        }

        // GET: /Teams
        [HttpGet]
        public async Task<ActionResult<List<Team>>> Get()
        {
            Console.WriteLine($"查询团队成员消息");

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
            //    return Ok(new List<Team>());
            //}
            //else
            //{
            //    // 不使用缓存，dosomething...
            //    // 1、查询团队
            //    IList<Team> teams = await _teamServiceClient.GetTeams();
            //    // 2、查询团队成员
            //    foreach (var team in teams)
            //    {
            //        IList<Member> members = await _memberServiceClient.GetMembersByTeamId(team.Id);
            //        team.Members = members;
            //    }
            //    return Ok(teams);
            //}
            #endregion

            // 1、查询团队
            IList<Team> teams = await _teamServiceClient.GetTeams();

            // 2、查询团队成员
            foreach (var team in teams)
            {
                IList<Member> members = await _memberServiceClient.GetMembersByTeamId(team.Id);
                team.Members = members;
            }

            return Ok(teams);
        }


        /// <summary>
        /// 添加团队和成员信息，Saga事务测试
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //[HttpPost]
        [HttpPost, SagaStart]
        public async Task<ActionResult> Post(string value)
        {
            Console.WriteLine("添加团队信息和成员信息...");
            // throw new a exception for test
            throw new Exception("异常测试 --> 事务入口", new Exception());

            // 1. 添加团队信息
            var team = new Team()
            {
                Id = Guid.NewGuid(),
                TeamName = "研发团队",
            };
            await _teamServiceClient.InsertAsync(team);

            // 2. 添加成员信息
            var member = new Member()
            {
                Id = Guid.NewGuid(),
                MemberName = "Leo",
                NickName = "Leo-1",
                TeamId = team.Id
            };
            await _memberServiceClient.InsertAsync(member);

            return Ok("添加成功...");
        }
    }
}
