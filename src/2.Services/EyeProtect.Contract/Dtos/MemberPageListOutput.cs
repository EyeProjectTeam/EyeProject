using EyeProtect.Core.Enums;
using EyeProtect.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Dtos
{
    public class MemberPageListOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        public AccountType? AccountType { get; set; }

        /// <summary>
        /// 使用情况
        /// </summary>
        public string AccountTypeMsg => AccountType.DisplayName();
    }
}
