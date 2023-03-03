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
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using EyeProtect.CoreWeb.Filters;
using Microsoft.AspNetCore.Mvc;
using EyeProtect.Core.Cache.Commons;
using EyeProtect.Manage.Timers;

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
            (GroupName.ManagerApi, "爱眼管理端接口文档"),
            (GroupName.EngineApi, "爱眼引擎端接口文档")
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
            services.AddMvc().AddNewtonsoftJson(jsonOptions =>
            {
                jsonOptions.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                jsonOptions.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                jsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            }).AddApplicationPart(typeof(EyeProtectManagerApiModule).Assembly);

            services.AddHostedService<TimerService>();

            // Configure MVC
            //Configure<MvcOptions>(p =>
            //{
            //    p.Filters.Add<HttpOperationRecordFilters>();
            //});

            //auth policy
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminAuth", policy => policy.RequireAssertion(context =>
            //    {
            //        return context.User.IsInRole("Admin");
            //    }));
            //    options.AddPolicy("SupAdmin", policy => policy.RequireAssertion(context =>
            //    {
            //        context.User.HasClaim(claim => claim.Value)
            //    }));
            //});

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
            app.UseUnitOfWork();
            app.UseMiddleware<HttpOperationRecord>();
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
