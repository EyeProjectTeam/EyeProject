using Autofac;
using AutoMapper.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.AspNetCore;
using Volo.Abp.Autofac;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using EyeProtect.Core.Options;
using EyeProtect.Core.Cache.Commons;

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

            // OAuth
            var oauthSection = config.GetSection("Jwt");
            if (!oauthSection.Exists()) throw new ArgumentNullException(nameof(oauthSection));

            services.Configure<JwtOptions>(oauthSection);
            services.Configure<SupAdminOptions>(config.GetSection("SuperAdmin"));
            services.Configure<AccountLockOptions>(config.GetSection("AccountLock"));

            // Cache，默认缓存时间：30分钟， 内存+Redis
            Configure<CacheOptions>(options =>
            {
                options.CacheOption(CacheOptions.DefaultCacheName, p =>
                {
                    p.AbsoluteExpire = TimeSpan.FromMinutes(30);
                    p.StoragePolicy = CacheStoragePolicy.Memory;
                });
            });
        }

        private static bool _staticMapperInitialized;
        private static readonly object _autoMapperSyncObj = new object();
        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            // AutoMapper
            var config = new MapperConfigurationExpression();
            var actions = services.GetObject<AutoMapperConfig>()?.MapActions;
            if (actions != null)
            {
                foreach (var action in actions)
                {
                    action(config);
                }
            }
            lock (_autoMapperSyncObj)
            {
                if (!_staticMapperInitialized)
                {
                    _staticMapperInitialized = true;
                    Mapper.Initialize(config);
                }
            }
            services.AddSingleton(Mapper.Instance);
            services.RemoveAll<IObjectAccessor<AutoMapperConfig>>();

        }
    }
}