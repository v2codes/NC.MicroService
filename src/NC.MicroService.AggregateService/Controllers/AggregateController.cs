using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NC.MicroService.AggregateService.Domain;
using NC.MicroService.AggregateService.Services;

namespace NC.MicroService.AggregateService.Controllers
{
    [Route("api/TeamInfos")]
    [ApiController]
    public class AggregateController : ControllerBase
    {

        private readonly ITeamServiceClient _teamServiceClient;
        private readonly IMemberServiceClient _memberServiceClient;

        private readonly ILogger<AggregateController> _logger;

        public AggregateController(ITeamServiceClient teamServiceClient,
                                   IMemberServiceClient memberServicesClient,
                                   ILogger<AggregateController> logger)
        {
            this._teamServiceClient = teamServiceClient;
            this._memberServiceClient = memberServicesClient;
            this._logger = logger;
        }

        // GET: api/TeamInfos
        [HttpGet]
        public async Task<ActionResult<List<Team>>> Get()
        {
            Console.WriteLine($"查询团队成员消息");
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
    }
}
