using EyeProject.Core.Dto;
using EyeProtect.Application;
using EyeProtect.Core.Const;
using EyeProtect.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace EyeProtect.Manage.Controllers
{
    /// <summary>
    /// 引擎端接口
    /// </summary>
    [Route(ServiceRoute.EnginePath + "/[controller]")]
    [ApiExplorerSettings(GroupName = GroupName.EngineApi)]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;

        /// <summary>
        /// ctor
        /// </summary>
        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<Result<loginOuput>> Login([FromBody] loginInput input)
        {
            return await _memberService.LoginAsync(input);
        }
    }
}
