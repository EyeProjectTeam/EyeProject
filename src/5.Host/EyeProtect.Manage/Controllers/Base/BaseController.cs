using EyeProtect.Core.Const;
using Microsoft.AspNetCore.Mvc;

namespace EyeProtect.Manage.Controllers.Base
{
    [Route(ServiceRoute.ManagePath + "/[controller]")]
    [ApiExplorerSettings(GroupName = GroupName.ManagerApi)]
    public class BaseController : Controller
    {

    }
}
