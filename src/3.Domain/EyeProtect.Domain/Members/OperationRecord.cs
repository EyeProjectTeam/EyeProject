using System.ComponentModel.DataAnnotations;
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
        /// ctor
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ip"></param>
        /// <param name="operrationType"></param>
        public OperationRecord(string account, string ip, OperrationType operrationType)
        {
            Account = account;
            Ip = ip;
            OperrationType = operrationType;
        }


        /// <summary>
        /// 账号
        /// </summary>
        [StringLength(12)]
        public string Account { get; set; }

        /// <summary>
        /// IPAddress
        /// </summary>
        [StringLength(100)]
        public string Ip { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public OperrationType OperrationType { get; set; }


    }
}
