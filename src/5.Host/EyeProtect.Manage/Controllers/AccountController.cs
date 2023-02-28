using EyeProject.Core.Dto;
using EyeProtect.Application;
using EyeProtect.Core.Enums;
using EyeProtect.Dtos;
using EyeProtect.Manage.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EyeProtect.Manage.Controllers
{
    /// <summary>
    /// 账号服务
    /// </summary>
    public class AccountController : BaseController
    {
        private readonly ILogger<SystemController> _logger;
        private readonly IMemberService _memberService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="memberService"></param>
        public AccountController(ILogger<SystemController> logger, IMemberService memberService)
        {
            _logger = logger;
            _memberService = memberService;
        }

        /// <summary>
        /// 账号列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("MemberPageList")]
        public Task<PageResult<MemberPageListOutput>> GetMemberPageList(MemberPageListInput input)
        {
            return _memberService.GetMemberPageList(input);
        }

        /// <summary>
        /// 账号列表导出
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("ExportMemberList")]
        public Task<IActionResult> ExportMemberList(MemberPageListInput input)
        {
            var result = _memberService.ExportMemberList(input);
            return result;
        }

        /// <summary>
        /// 更新账号状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operateAccountType"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        [HttpPut("UpdateAccountType")]
        public Task<Result> UpdateAccountType([FromQuery] long id, [FromBody] OperateAccountType operateAccountType)
        {
            return _memberService.UpdateAccountType(id, operateAccountType);
        }

    }
}