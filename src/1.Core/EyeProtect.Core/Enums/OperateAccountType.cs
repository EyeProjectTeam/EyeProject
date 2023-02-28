using System.ComponentModel.DataAnnotations;

namespace EyeProtect.Core.Enums
{
    public enum OperateAccountType
    {
        [Display(Name = "出售")]
        Sale = 1,
        [Display(Name = "续费")]
        ReSale = 2
    }
}
