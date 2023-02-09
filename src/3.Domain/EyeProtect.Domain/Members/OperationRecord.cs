using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using EyeProtect.Core.Domain;
using EyeProtect.Core.Enums;

namespace EyeProtect.Members
{
    /// <summary>
    /// 操作记录
    /// </summary>
    public class OperationRecord : Entity<long>
    {
        /// <summary>
        /// 会员Id
        /// </summary>
        public long MemberId { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 会员姓名
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// IPAddress
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public OperrationType OperrationType { get; set; }


    }
}
