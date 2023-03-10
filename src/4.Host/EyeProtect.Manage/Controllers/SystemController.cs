using EyeProject.Core.Dto;
using EyeProtect.Application;
using EyeProtect.Contract.Dtos;
using EyeProtect.Core.Const;
using EyeProtect.Dtos;
using EyeProtect.Manage.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EyeProtect.Manage.Controllers
{
    /// <summary>
    /// 系统服务
    /// </summary>
    public class SystemController : BaseController
    {
        private readonly ILogger<SystemController> _logger;
        private readonly IMemberService _memberService;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="memberService"></param>
        public SystemController(ILogger<SystemController> logger, IMemberService memberService)
        {
            _logger = logger;
            _memberService = memberService;
        }

        /// <summary>
        /// 登录页
        /// </summary>
        /// <returns></returns>
        [HttpGet("Login"), AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("Login"), AllowAnonymous]
        public async Task<Result<loginOuput>> Login([FromBody] loginInput input)
        {
            return await _memberService.LoginAsync(input);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpPost("LogOut")]
        public Result LogOut()
        {
            HttpContext.Response.Cookies.Delete(EyeProtectConst.DefaultCookieName);
            return Result.Ok();
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var result = await _memberService.StaticAccountData();
            if (result.IsOk())
            {
                return View(result.Data);
            }
            else
            {
                return View(new StaticAccountDataOutput());
            }
        }

        /// <summary>
        /// 账号列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("MemberList")]
        public IActionResult MemberList()
        {
            return View();
        }

        /// <summary>
        /// 超时页
        /// </summary>
        /// <returns></returns>
        [HttpGet("TimeOut")]
        public IActionResult TimeOut()
        {
            return View();
        }
    }
}