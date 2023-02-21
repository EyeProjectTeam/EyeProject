using DeviceManage.Core;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace EyeProtect;

[DependsOn(typeof(EyeProtectCoreModule))]
public class EyeProtectDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }
}
