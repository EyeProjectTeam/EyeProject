using DeviceManage.Core;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace DeviceManage.CoreWeb
{
    [DependsOn(typeof(EyeProtectCoreModule))]
    public class EyeProtectCoreWebModule : AbpModule
    {
        /// <inheritdoc />
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var service = context.Services;

            //// MVC
            //service.AddMvc();

        }
    }
}