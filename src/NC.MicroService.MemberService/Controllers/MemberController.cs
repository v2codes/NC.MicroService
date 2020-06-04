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
        /// 根据团队ID查询所有成员信息
        /// </summary>
        /// <param name="teamId">团队ID</param>
        /// <returns></returns>
        [HttpGet("/Members/{teamId}")]
        public ActionResult<IEnumerable<Member>> GetMembers([FromRoute]Guid teamId)
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

        /// <summary>
        /// 根据ID查询成员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/Members/Get/{id}")]
        public ActionResult<Member> GetMember([FromRoute]Guid id)
        {
            var member = _memberService.Find(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        /// <summary>
        /// 更新成员信息
        /// </summary>
        /// <param name="id">成员ID</param>
        /// <param name="member">成员信息</param>
        /// <returns></returns>
        [HttpPut("/Members/{id}")]
        public IActionResult PutMember([FromRoute]Guid id, [FromBody]Member member)
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

            return Ok("更新成功");
        }

        /// <summary>
        /// 创建成员信息
        /// </summary>
        /// <param name="teamId">团队ID</param>
        /// <param name="member">成员信息</param>
        /// <returns></returns>
        [HttpPost("/Members/{teamId}")]
        public async Task<ActionResult<Member>> PostMember([FromRoute]Guid teamId, [FromBody]Member member)
        {
            member.TeamId = teamId;
            await _memberService.InsertAsync(member);

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("/Members/{id}")]
        public ActionResult<Member> DeleteMember([FromRoute]Guid id)
        {
            var member = _memberService.Find(id);
            if (member == null)
            {
                return NotFound();
            }
            _memberService.Delete(member);
            return member;
        }
    }

}
