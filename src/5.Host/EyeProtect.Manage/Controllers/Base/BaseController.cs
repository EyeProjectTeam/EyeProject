using EyeProtect.Core.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EyeProtect.Manage.Controllers.Base
{
    [Authorize]
    [Route(ServiceRoute.ManagePath + "/[controller]")]
    [ApiExplorerSettings(GroupName = GroupName.ManagerApi)]
    public class BaseController : Controller
    {

    }
}
