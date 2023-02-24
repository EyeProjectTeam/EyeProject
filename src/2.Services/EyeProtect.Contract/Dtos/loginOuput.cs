using EyeProtect.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Dtos
{
    public class loginOuput
    {
        public long Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        public AccountType AccountType { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsAvailable => new[] { AccountType.UnUse, AccountType.Expire }.Contains(AccountType);
    }
}
