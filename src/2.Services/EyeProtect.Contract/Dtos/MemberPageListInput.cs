using EyeProject.Core.Dto;
using EyeProtect.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Dtos
{
    public class MemberPageListInput : PageQuery
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Query(QueryCompare.Equal)]
        public string Account { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        public AccountType? AccountType { get; set; }
    }
}
