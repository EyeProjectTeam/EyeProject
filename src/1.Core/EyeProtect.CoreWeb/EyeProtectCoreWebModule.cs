using DeviceManage.Core;
using EyeProtect.CoreWeb.Filters;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Modularity;

namespace DeviceManage.CoreWeb
{
    [DependsOn(typeof(EyeProtectCoreModule))]
    public class EyeProtectCoreWebModule : AbpModule
    {
        /// <inheritdoc />
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            // Configure MVC
            Configure<MvcOptions>(p =>
            {
                p.Filters.Add<ExceptionResultFilter>();
                p.Filters.Add<HttpLogFilter>();
            });

            /// <summary>
            /// 阻止跨站点请求伪造
            /// https://docs.microsoft.com/zh-cn/aspnet/core/security/anti-request-forgery?view=aspnetcore-6.0
            /// </summary>
            Configure<AbpAntiForgeryOptions>(options => { options.AutoValidate = false; });
        }
    }
}