using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NC.MicroService.TeamService.Domain;
using NC.MicroService.TeamService.Services;

namespace NC.MicroService.TeamService.Controllers
{
    [Route("Teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private ITeamService _teamService;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
        {
            _teamService = teamService;
            _logger = logger;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Team>> GetTeams()
        {
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
