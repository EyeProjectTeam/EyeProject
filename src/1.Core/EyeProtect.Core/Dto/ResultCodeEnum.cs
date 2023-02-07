using System.ComponentModel.DataAnnotations;

namespace EyeProject.Core.Dto
{
    /// <summary>
    /// Api请求状态码
    /// </summary>
    public enum ResultCodeEnum
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
        /// 调用频次超限
        ///</summary>
        [Display(Name = "调用频次超限")]
        CallLimited = 5,

        /// <summary>
        /// 服务数据异常
        ///</summary>
        [Display(Name = "服务数据异常")]
        ServerError = 10,

        /// <summary>
        /// 未登录
        ///</summary>
        [Display(Name = "未登录")]
        Unauthorized = 20,

        /// <summary>
        /// 未授权
        /// </summary>
        [Display(Name = "未授权")]
        Forbidden = 21,

        /// <summary>
        /// Token 失效
        /// </summary>
        [Display(Name = "Token 失效")]
        InvalidToken = 22,

        /// <summary>
        /// 密码验证失败
        /// </summary>
        [Display(Name = "密码验证失败")]
        SpaFailed = 23,

        /// <summary>
        /// 错误的新密码
        /// </summary>
        [Display(Name = "错误的新密码")]
        WrongNewPassword = 24,

        /// <summary>
        /// 参数验证失败
        /// </summary>
        [Display(Name = "参数验证失败")]
        InvalidData = 403,

        /// <summary>
        /// 没有此条记录
        ///</summary>
        [Display(Name = "没有此条记录")]
        NoRecord = 404,

        /// <summary>
        /// 重复记录
        /// </summary>
        [Display(Name = "重复记录")]
        DuplicateRecord = 405,

        /// <summary>
        /// 缺失基础数据
        /// </summary>
        [Display(Name = "缺失基础数据")]
        MissEssentialData = 406,
    }
}
