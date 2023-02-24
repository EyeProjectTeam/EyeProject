using EyeProject.Core.Dto;
using EyeProtect.Application;
using EyeProtect.Core.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EyeProtect.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route(ServiceRoute.ManagePath + "/[controller]")]
    [ApiExplorerSettings(GroupName = GroupName.ManagerApi)]

    public class SupAdminController : Controller
    {
        private readonly ILogger<SupAdminController> _logger;
        private readonly IMemberService _memberService;

        public SupAdminController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// 生成账号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpPost("MakeAccount")]
        public Task<Result> MakeAccount([FromQuery] int number)
        {
            return _memberService.MakeAccount(number);
        }
    }
}
