using EyeProtect.Core.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace EyeProtect.Manager.Controllers.Base
{
    [Authorize]
    [Route(ServiceRoute.ManagePath + "/[controller]")]
    [ApiExplorerSettings(GroupName = GroupName.ManagerApi)]
    public class BaseController : AbpControllerBase
    {
    }
}
