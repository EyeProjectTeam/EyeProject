using EyeProject.Core.Dto;
using EyeProtect.Application;
using EyeProtect.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EyeProtect.Manage.Controllers
{
    public class SystemController : Controller
    {
        private readonly ILogger<SystemController> _logger;
        private readonly IMemberService _memberService;

        public SystemController(ILogger<SystemController> logger, IMemberService memberService)
        {
            _logger = logger;
            _memberService = memberService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<Result<loginOuput>> Login(loginInput input)
        {
            return await _memberService.LoginAsync(input);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccountList()
        {
            return View();
        }
    }
}