using DeviceManage.Core;
using EyeProtect.CoreWeb.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
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
                p.Filters.Add<ValidateResultFilter>();
                p.Filters.Add<HttpLogFilter>();
            });

            /// <summary>
            /// 阻止跨站点请求伪造
            /// https://docs.microsoft.com/zh-cn/aspnet/core/security/anti-request-forgery?view=aspnetcore-6.0
            /// </summary>
            Configure<AbpAntiForgeryOptions>(options => { options.AutoValidate = false; });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            app.UseMiddleware<HandlingResultMiddleware>();
            app.UseMiddleware<HttpLogMiddleware>();
        }
    }
}