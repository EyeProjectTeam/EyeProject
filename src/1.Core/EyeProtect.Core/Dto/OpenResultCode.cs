using System.ComponentModel.DataAnnotations;

namespace EyeProject.Core.Dto
{
    /// <summary>
    /// Open项目返回值
    /// </summary>
    public enum OpenResultCode
    {
        /// <summary>
        /// 操作成功
        ///</summary>
        [Display(Name = "操作成功")]
        Ok = 0,

        /// <summary>
        /// 操作失败
        ///</summary>
        [Display(Name = "操作失败")]
        Fail = 1,

        /// <summary>
        /// 当前工号不存在
        /// </summary>
        [Display(Name = "当前工号不存在")]
        NoRecord = 201,

        /// <summary>
        /// token验证不通过
        ///</summary>
        [Display(Name = "token验证不通过")]
        IlLegalToken = 202,

        /// <summary>
        /// 请求参数不正确
        ///</summary>
        [Display(Name = "请求参数不正确")]
        IlLegalParam = 203,

        /// <summary>
        /// 请求参数不正确
        ///</summary>
        [Display(Name = "请求参数不正确")]
        IlLegalParam2 = 204,

        /// <summary>
        /// 系统异常
        ///</summary>
        [Display(Name = "系统异常")]
        SystemExcept = 205,

        /// <summary>
        /// 工号不存在！
        ///</summary>
        [Display(Name = "工号不存在！")]
        EmployeeNotExist = 206,

        /// <summary>
        /// 密码不正确！
        ///</summary>
        [Display(Name = "密码不正确！")]
        PasswordError = 207
    }
}
