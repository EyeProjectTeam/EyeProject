using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Core.Enums
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperrationType
    {
        [Display(Name = "登入")]
        Login = 1,

        [Display(Name = "登入密码错误")]
        PwdError = 1,

        [Display(Name = "登出")]
        Logout = 2,

        [Display(Name = "引擎")]
        Engine = 3,

        [Display(Name = "管理端")]
        Manager = 4
    }
}
