using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NC.MicroService.MemberService.Domain;
using NC.MicroService.MemberService.Services;

namespace NC.MicroService.MemberService.Controllers
{
    [Route("Members")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly ILogger<MemberController> _logger;

        public MemberController(IMemberService memberService, ILogger<MemberController> logger)
        {
            _memberService = memberService;
            _logger = logger;
        }

        /// <summary>
        /// 查询所有成员信息
        /// </summary>
        /// <param name="teamId">?teamId参数结尾方式</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Member>> GetMembers(Guid teamId)
        {
            if (teamId == null || teamId == Guid.Empty)
            {
                return _memberService.QueryAll();
            }
            else
            {
                return _memberService.Query(p => p.TeamId == teamId).ToList();
            }
        }


        // GET: api/Members/5
        [HttpGet("{id}")]
        public ActionResult<Member> GetMember(Guid id)
        {
            var member = _memberService.Find(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public IActionResult PutMember(Guid id, Member member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }

            try
            {
                _memberService.Update(member);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_memberService.Exists(p => p.Id == id))
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

        // POST: api/Members
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public ActionResult<Member> PostMember(Guid teamId, Member member)
        {
            member.TeamId = teamId;
            _memberService.Insert(member);

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public ActionResult<Member> DeleteMember(Guid id)
        {
            var member = _memberService.Find(id);
            if (member == null)
            {
                return NotFound();
            }
            _memberService.Delete(id);

            return member;
        }
    }

}
