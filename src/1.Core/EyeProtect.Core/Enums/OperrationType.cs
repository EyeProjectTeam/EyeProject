using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EyeProtect.Core.Enums
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperrationType
    {
        #region 登录
        [Display(Name = "登入")]
        Login = 1,

        [Display(Name = "登入密码错误")]
        PwdError = 2,

        [Display(Name = "登出")]
        Logout = 3,

        [Display(Name = "引擎")]
        Engine = 4,

        [Display(Name = "管理端")]
        Manager = 5,
        #endregion

        #region 操作

        [Display(Name = "续期")]
        ReSale = 6

        #endregion
    }
}
