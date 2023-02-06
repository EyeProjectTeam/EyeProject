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
    }
}