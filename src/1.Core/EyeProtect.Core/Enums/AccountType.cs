﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EyeProtect.Core.Enums
{
    /// <summary>
    /// 账号类型
    /// </summary>
    public enum AccountType
    {
        [Display(Name = "已使用")]
        Used = 0,
        [Display(Name = "已过期")]
        Expire = 1,
        [Display(Name = "未使用")]
        UnUse = 2,
        [Display(Name = "未出售")]
        UnSale = 3,
    }
}
