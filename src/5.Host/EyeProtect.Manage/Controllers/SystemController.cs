using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EyeProtect.Manage.Controllers
{
    public class SystemController : Controller
    {
        private readonly ILogger<SystemController> _logger;

        public SystemController(ILogger<SystemController> logger)
        {
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost("")]
        //public async Task<Results<>> Login()
        //{
        //    Response.Redirect("/monitor");
        //    return View();
        //}

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