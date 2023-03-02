using EyeProject.Core.Dto;
using EyeProtect.Core.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace EyeProtect.CoreWeb.Filters
{
    /// <summary>
    /// 防止浏览器直接补
    /// </summary>
    public class HandlingResultMiddleware : IMiddleware, ITransientDependency
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            //catch (Exception ex)
            //{
            //    var statusCode = context.Response.StatusCode;
            //    if (ex is ArgumentException)
            //    {
            //        statusCode = ResultCode.InvalidData;
            //    }
            //    await HandleExceptionAsync(context, (statusCode, ex.Message));
            //}
            finally
            {
                var statusCode = context.Response.StatusCode;
                if (statusCode == (int)HttpStatusCode.Unauthorized || statusCode == (int)HttpStatusCode.Forbidden)
                {
                    await HandleExceptionAsync(context, (statusCode, ResultCode.Forbidden.DisplayName()));
                }
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Result result)
        {
            context.Response.ContentType = "application/json;charset=utf-8";
            return context.Response.WriteAsync(result.ToJson());
        }
    }
}
