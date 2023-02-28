using EyeProject.Core.Dto;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using EyeProtect.Core.Exceptions;
using Microsoft.Extensions.Hosting;
using EyeProtect.Core.Utils;
using Volo.Abp.DependencyInjection;

namespace EyeProtect.CoreWeb.Filters
{
    /// <summary>
    /// 异常返回信息
    /// </summary>
    public class ExceptionResultFilter : IAsyncExceptionFilter, ITransientDependency
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebHostEnvironment _env;

        /// <inheritdoc />
        public ExceptionResultFilter(IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            _env = env;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is OperationCanceledException ||
               context.Exception is ThreadAbortException) return;

            var exception = context.Exception;
            IResult result;

            if (exception is ResultException resultException)
            {
                result = resultException.Result;
            }
            else
            {
                var errorResult = new ErrorResult();
                errorResult.ByError("系统异常，请稍后再试！");

                if (!_env.IsProduction())
                {
                    errorResult.Error[nameof(Exception.Message)] = exception.GetAllMessage();
                }

                result = errorResult;
            }

            if (context.Result == null)
            {
                var objResult = new ObjectResult(result);
                if (result.Code == ResultCode.Unauthorized)
                {
                    objResult.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else if (result.Code == ResultCode.Forbidden)
                {
                    objResult.StatusCode = (int)HttpStatusCode.Forbidden;
                }

                context.Result = objResult;
            }

            context.HttpContext.Items["ActionException"] = exception;
            context.HttpContext.Items["ActionResult"] = context.Result;
            context.ExceptionHandled = true;
        }
    }
}
