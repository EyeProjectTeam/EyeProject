using DeviceManage.CoreWeb;
using EyeProtect.Core.Const;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace EyeProtect.Manager
{
    [DependsOn(typeof(EyeProtectCoreWebModule))]
    [DependsOn(typeof(EyeProtectServiceModule))]
    public class EyeProtectManagerApiModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

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

            services.AddHttpContextAccessor();
            services.AddObjectAccessor<IApplicationBuilder>();

            //JWT
            ConfigureJwtAuthentication(context, config);

            //MvcAndCors
            ConfigureMvcCors(context, config);
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

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(DefaultCorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseConfiguredEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                // MVC
                endpoints.MapDefaultControllerRoute();
            });
        }

        #region Method

        /// <summary>
        /// 配置JWT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        private void ConfigureJwtAuthentication(ServiceConfigurationContext context, IConfiguration config)
        {
            context.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters =
                                new TokenValidationParameters()
                                {
                                    // 是否开启签名认证
                                    ValidateIssuerSigningKey = true,
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    //ClockSkew = TimeSpan.Zero,
                                    ValidIssuer = config["Jwt:Issuer"],
                                    ValidAudience = config["Jwt:Audience"],
                                    IssuerSigningKey = new SymmetricSecurityKey(
                                            Encoding.ASCII.GetBytes(config["Jwt:SecurityKey"]))
                                };

                            options.Events = new JwtBearerEvents
                            {
                                OnMessageReceived = currentContext =>
                                {
                                    var path = currentContext.HttpContext.Request.Path;
                                    if (path.StartsWithSegments("/login"))
                                    {
                                        return Task.CompletedTask;
                                    }

                                    var accessToken = string.Empty;
                                    if (currentContext.HttpContext.Request.Headers.ContainsKey("Authorization"))
                                    {
                                        accessToken = currentContext.HttpContext.Request.Headers["Authorization"];
                                        if (!string.IsNullOrWhiteSpace(accessToken))
                                        {
                                            accessToken = accessToken.Split(" ").LastOrDefault();
                                        }
                                    }

                                    if (accessToken.IsNullOrWhiteSpace())
                                    {
                                        accessToken = currentContext.Request.Query["access_token"].FirstOrDefault();
                                    }

                                    if (accessToken.IsNullOrWhiteSpace())
                                    {
                                        accessToken = currentContext.Request.Cookies[EyeProtectConst.DefaultCookieName];
                                    }

                                    currentContext.Token = accessToken;
                                    currentContext.Request.Headers.Remove("Authorization");
                                    currentContext.Request.Headers.Add("Authorization", $"Bearer {accessToken}");

                                    return Task.CompletedTask;
                                }
                            };
                        });
        }

        /// <summary>
        /// 配置跨域
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        private void ConfigureMvcCors(ServiceConfigurationContext context, IConfiguration config)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            config["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // Routing
            context.Services.AddRouting();

            // MVC
            context.Services.AddMvc()
                .AddApplicationPart(typeof(EyeProtectManagerApiModule).Assembly)
                .AddControllersAsServices();
        }

        #endregion
    }
}
