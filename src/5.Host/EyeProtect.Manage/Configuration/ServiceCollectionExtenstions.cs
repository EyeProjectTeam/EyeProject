using EyeProject.Core.Dto;
using EyeProtect.Core.Const;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;

namespace EyeProtect.Manage.Configuration
{
    public static class ServiceCollectionExtenstions
    {
        /// <summary>
        /// 添加Swagger配置
        /// </summary>
        public static void AddSwagger(this IServiceCollection services, IConfiguration config,
            (string Name, string Title)[] docs)
        {
            // Api文档
            services.AddSwaggerGen(option =>
            {

                foreach (var (name, title) in docs)
                {
                    option.SwaggerDoc(name, new OpenApiInfo { Title = title, Version = "v1" });
                }

                var dic = Path.GetDirectoryName(typeof(EyeProtectManagerApiModule).Assembly.Location);
                foreach (var file in Directory.EnumerateFiles(dic, "*.xml"))
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName.StartsWith("EyeProtect"))
                    {
                        option.IncludeXmlComments(file, true);
                    }
                }

                option.DocInclusionPredicate((docName, api) => api.GroupName == docName);
                option.IgnoreObsoleteActions();
                //option.SchemaGeneratorOptions.CustomTypeMappings.Add(typeof(IFile), () => FileSchemaFilter.Schema);
                //option.SchemaFilter<EnumSchemaFilter>();
                //option.SchemaFilter<FileSchemaFilter>();
                //option.SchemaFilter<ResultCodeToEnumSchemaFilter>();

                // 接口安全验证                
                //option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                //{
                //    Type = SecuritySchemeType.OAuth2,
                //    Flows = new OpenApiOAuthFlows()
                //    {

                //        ClientCredentials = new OpenApiOAuthFlow()
                //        {
                //            TokenUrl = new Uri($"{config.GetValue<string>("OAuth:Authority")}connect/token"),
                //            Scopes =
                //            {
                //                {ApiScopes.TicketWindowCommonApi, ApiScopes.TicketWindowCommonApi}
                //            }
                //        }
                //    }
                //});

                option.AddSecurityDefinition("bearer", new OpenApiSecurityScheme()
                {
                    Description = "Authorization format : Bearer {token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                var authOptions = new AuthorizationOptions();
                //option.OperationFilter<AuthPolicyScopSecurityRequirementsFilter>(authOptions);
            }).AddSwaggerGenNewtonsoftSupport();
        }

        /// <summary>
        /// 配置JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
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
        /// <param name="config"></param>
        public static void ConfigureCors(this IServiceCollection services, IConfiguration config, string defaultCorsPolicyName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(defaultCorsPolicyName, builder =>
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
        }
    }
}
