using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace DeviceManage.Core
{
    /// <summary>
    /// 公共模块
    /// </summary>
    public class EyeProtectCoreModule : AbpModule
    {
        /// <inheritdoc />
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var config = context.Services.GetConfiguration();

            //配置注入
            services.AddSingleton(config);

        }
    }
}