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
        public long Id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [ExcelExporter(2, "账号")]
        public string Account { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        public AccountType? AccountType { get; set; }

        /// <summary>
        /// 使用情况
        /// </summary>
        [ExcelExporter(2, "使用情况")]
        public string AccountTypeMsg => AccountType.DisplayName();
    }
}
