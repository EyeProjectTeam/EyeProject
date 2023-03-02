using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Core.Options
{
    public class AccountLockOptions
    {
        /// <summary>
        /// 锁定次数
        /// </summary>
        public int LockNum { get; set; }

        /// <summary>
        /// 锁定时间（分钟）
        /// </summary>
        public int LockTime { get; set; }

        /// <summary>
        /// 锁定时间范围(分钟)
        /// </summary>
        public int LockRange { get; set; }
    }
}
