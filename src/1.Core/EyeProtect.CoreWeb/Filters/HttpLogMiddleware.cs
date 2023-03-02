using EyeProtect.Core.Utils;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Extensions;

namespace EyeProtect.CoreWeb.Filters
{
    public class HttpLogMiddleware : IMiddleware, ITransientDependency
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly HttpLogOptions _options;

        /// <inheritdoc cref="HttpLogMiddleware" />
        public HttpLogMiddleware(ILoggerFactory loggerFactory, IOptions<HttpLogOptions> options)
        {
            _loggerFactory = loggerFactory;
            _options = options.Value;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;
            var headers = context.Request.GetTypedHeaders();
            var requestBody = string.Empty;
            var responseBody = string.Empty;
            Exception exception = null;

            var watch = Stopwatch.StartNew();

            try
            {
                if (headers.ContentType != null && _options.LogMediaTypes.Any(p => headers.ContentType.IsSubsetOf(p)))
                {
                    if (!context.Request.Body.CanSeek)
                    {
                        context.Request.EnableBuffering();
                        await request.Body.DrainAsync(CancellationToken.None);
                        request.Body.Seek(0L, SeekOrigin.Begin);
                    }

                    var encoding = headers.ContentType.Encoding ?? SelectCharacterEncoding(request.ContentType);
                    using (var reader = new StreamReader(request.Body, encoding, false, 4096, true))
                    {
                        requestBody = await reader.ReadToEndAsync();
                    }

                    request.Body.Seek(0L, SeekOrigin.Begin);
                }

                await next(context);
                watch.Stop();
            }
            catch (Exception e)
            {
                exception = e;
                e.ReThrow();
            }
            finally
            {
                watch.Stop();

                if (context.Items.TryGetValue("ActionDescriptor", out var c1) &&
                    c1 is ControllerActionDescriptor actionDescriptor)
                {
                    var logName = $"{actionDescriptor.ControllerTypeInfo.FullName}.{actionDescriptor.MethodInfo.Name}";
                    var logger = _loggerFactory.CreateLogger(logName);

                    var executedContext = context.Items.TryGetValue("ActionExecutedContext", out var c3)
                        ? c3 as ActionExecutedContext : null;
                    if (executedContext?.Result is ObjectResult result && !(result.Value is byte[] || result.Value is Stream))
                    {
                        responseBody = JsonConvert.SerializeObject(result.Value, _options.SerializerSettings);
                    }

                    var userId = context.User.FindFirst(JwtClaimTypes.Subject)?.Value;
                    var userName = context.User.FindFirst(JwtClaimTypes.Name)?.Value;
                    var name = context.User.FindFirst(JwtClaimTypes.NickName)?.Value;
                    var clientId = context.User.FindFirst(JwtClaimTypes.ClientId)?.Value;

                    if (exception == null && context.Items.TryGetValue("ActionException", out var ex))
                    {
                        exception = ex as Exception;
                    }

                    var logLevel = exception != null ? LogLevel.Error : LogLevel.Information;

                    logger.Log(logLevel, exception,
                        "请求：{Method} {RequestUrl} {Protocol} {RequestContentType}\r\n" +
                        "用户：UserName={UserName} Name={Name} UserId={UserId} ClientId={ClientId}\r\n" +
                        "{RequestBody}\r\n" +
                        "响应：{ElapsedMilliseconds}ms {StatusCode} {ResponseContentType}\r\n" +
                        "{ResponseBody}\r\n" +
                        "{ErrorMessage}",
                        context.Request.Method,
                        context.Request.GetDisplayUrl(),
                        context.Request.Protocol,
                        headers.ContentType?.ToString() ?? string.Empty,
                        userName ?? string.Empty, name ?? string.Empty,
                        userId ?? string.Empty, clientId ?? string.Empty,
                        requestBody,
                        watch.Elapsed.TotalMilliseconds,
                        context.Response.StatusCode,
                        context.Response.ContentType,
                        responseBody,
                        exception?.GetAllMessage() ?? string.Empty);
                }
            }
        }



        private Encoding SelectCharacterEncoding(string contentType)
        {
            var mediaType = contentType == null ? new MediaType() : new MediaType(contentType);

            return mediaType.Encoding ?? Encoding.ASCII;
        }

        /// <summary>
        /// 接口请求日志配置
        /// </summary>
        public class HttpLogOptions
        {
            /// <summary>
            /// 接口请求日志配置
            /// </summary>
            public HttpLogOptions()
            {
                LogMediaTypes = new List<MediaTypeHeaderValue>()
            {
                new MediaTypeHeaderValue("text/json"),
                new MediaTypeHeaderValue("text/xml"),
                new MediaTypeHeaderValue("application/json"),
                new MediaTypeHeaderValue("application/json-patch+json"),
                new MediaTypeHeaderValue("application/*+json"),
                new MediaTypeHeaderValue("application/x-www-form-urlencoded"),
            };

                SerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver(),
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Include,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    Converters =
                {
                    new IsoDateTimeConverter
                    {
                        DateTimeFormat = "yyyy-MM-dd HH:mm:ss",
                        DateTimeStyles = DateTimeStyles.AssumeLocal
                    }
                }
                };
            }

            /// <summary>
            /// 参数Json序列化配置
            /// </summary>
            public JsonSerializerSettings SerializerSettings { get; }

            /// <summary>
            /// 要记录的 Body 类型
            /// </summary>
            public IList<MediaTypeHeaderValue> LogMediaTypes { get; }
        }
    }
}
