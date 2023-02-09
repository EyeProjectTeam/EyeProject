using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EyeProtect.Core.Domain;
using EyeProtect.Members;
using EyeProtect.Core.Enums;

namespace EyeProtect.Domain.Members
{
    /// <summary>
    /// 会员信息
    /// </summary>
    public class Member : Entity<long>
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(11)]
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(50)]
        public string Email { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        public AccountType? AccountType { get; set; }

        /// <summary>
        /// 操作记录
        /// </summary>
        public virtual List<OperationRecord> OperationRecords { get; set; }
    }
}
