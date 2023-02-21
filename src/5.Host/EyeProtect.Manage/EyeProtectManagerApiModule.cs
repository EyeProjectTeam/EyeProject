using DeviceManage.CoreWeb;
using EyeProtect.Core.Const;
using EyeProtect.Manage.Configuration;
using EyeProtect.Manage.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace EyeProtect.Manage
{
    [DependsOn(typeof(EyeProtectCoreWebModule))]
    [DependsOn(typeof(EyeProtectServiceModule))]
    public class EyeProtectManagerApiModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        /// <summary>
        /// Swagger文档
        /// </summary>
        private readonly (string Name, string Title)[] SwaggerDocs =
        {
            (GroupName.ManagerApi, "爱眼接口文档")
        };

        public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
        {
            // 应用程序初始化的时候注册hangfire
            //context.CreateRecurringJob();
            base.OnPostApplicationInitialization(context);
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var config = context.Services.GetConfiguration();

            //JWT
            services.ConfigureJwtAuthentication(config);

            //Cors
            services.ConfigureCors(config, DefaultCorsPolicyName);

            // Routing
            services.AddRouting();

            services.AddControllersWithViews();

            // MVC
            services.AddMvc()
                .AddApplicationPart(typeof(EyeProtectManagerApiModule).Assembly);

            // Swagger
            services.AddSwagger(config, SwaggerDocs);
        }

        /// <inheritdoc />
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var config = context.ServiceProvider.GetRequiredService<IConfiguration>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseAbpSecurityHeaders();
            app.UseRouting();
            app.UseCors(DefaultCorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<HttpOperationRecord>();
            app.UseUnitOfWork();
            app.UseEndpoints(endpoints =>
            {
                // MVC
                endpoints.MapDefaultControllerRoute();
            });

            app.UseSwaggerDashboard(config, ServiceRoute.ManagePath, SwaggerDocs);
        }

        #region Method





        #endregion
    }
}
