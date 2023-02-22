using EyeProtect.Core.Enums;
using EyeProtect.Core.Utils;
using Mutone.Core.Utils.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Dtos
{
    public class ExportMemberListInput
    {
        [ExcelExporter(2, "主键")]
        public long Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [ExcelExporter(3, "账号")]
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [ExcelExporter(4, "密码")]
        public string Password { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        public AccountType? AccountType { get; set; }

        /// <summary>
        /// 使用情况
        /// </summary>
        [ExcelExporter(5, "使用情况")]
        public string AccountTypeMsg => AccountType.DisplayName();
    }
}
