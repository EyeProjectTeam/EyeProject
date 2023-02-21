using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.AspNetCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DeviceManage.Core
{
    [DependsOn(
    typeof(AbpAspNetCoreModule),
    typeof(AbpAutofacModule))]
    public class EyeProtectCoreModule : AbpModule
    {
        /// <inheritdoc />
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var config = context.Services.GetConfiguration();

            //配置注入
            services.AddSingleton(config);

            // OAuth
            var oauthSection = config.GetSection("Jwt");
            if (!oauthSection.Exists()) throw new ArgumentNullException(nameof(oauthSection));

            services.Configure<OAuthOptions>(oauthSection);
        }
    }
}