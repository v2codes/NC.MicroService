using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public MemberController(IMemberService memberService,
                                IConfiguration configuration) // 演示查看配置中心是否正常 
        {
            _memberService = memberService;
            _configuration = configuration;
        }

        /// <summary>
        /// 查询所有成员信息
        /// </summary>
        /// <param name="teamId">?teamId参数结尾方式</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Member>> GetMembers(Guid teamId)
        {

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
            //    return new List<Member>();
            //}
            //else
            //{
            //    // 不使用缓存，dosomething...
            //    return _memberService.QueryAll();
            //}
            #endregion

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
