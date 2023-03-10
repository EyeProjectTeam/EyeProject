using EyeProject.Core.Dto;
using EyeProtect.Core.Enums;
using EyeProtect.Core.Utils;
using EyeProtect.Dtos;
using EyeProtect.Members;
using EyeProtect.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace EyeProtect.Manage.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpOperationRecord : IMiddleware, ITransientDependency
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOperationRecordRepository _operationRecordRepository;
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="operationRecordRepository"></param>
        public HttpOperationRecord(IOperationRecordRepository operationRecordRepository, ICurrentUser currentUser, ILoggerFactory loggerFactory)
        {
            _operationRecordRepository = operationRecordRepository;
            _currentUser = currentUser;
            _loggerFactory = loggerFactory;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;
            var headers = context.Request.GetTypedHeaders();
            var requestBody = string.Empty;
            try
            {
                if (headers.ContentType != null)
                {
                    if (!request.Body.CanSeek)
                    {
                        request.EnableBuffering();
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
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Throw(e);
            }
            finally
            {
                if (context.Items.TryGetValue("ActionDescriptor", out var c1) && c1 is ControllerActionDescriptor actionDescriptor)
                {
                    var executedContext = context.Items.TryGetValue("ActionExecutedContext", out var c3) ? c3 as ActionExecutedContext : null;
                    if (executedContext?.Result is ObjectResult objectResult && !(objectResult.Value is byte[] || objectResult.Value is Stream) && objectResult.Value is Result result)
                    {
                        var ip = context.Connection.RemoteIpAddress?.ToString();
                        var operationRecords = HandleOperationRecord(actionDescriptor, result.Code, ip, requestBody, objectResult.Value.ToJson());
                        await _operationRecordRepository.InsertManyAsync(operationRecords);
                    }
                }
            }
        }

        #region Method

        private IList<OperationRecord> HandleOperationRecord(ControllerActionDescriptor actionDescriptor, int code, string ip, string requestBody, string responseBody)
        {
            var account = string.Empty;
            var claims = _currentUser.GetAllClaims();
            if (claims != null && claims.Any())
            {
                account = claims.GetClaimValue(AbpClaimTypes.UserId);
            }
            var operationRecords = new List<OperationRecord>();
            switch (actionDescriptor.ActionName.ToLower())
            {
                case "login":
                    if (code == ResultCode.Ok)
                    {
                        var responseModel = JsonConvert.DeserializeObject<Result<loginOuput>>(responseBody).Data;
                        account = responseModel.Account;
                        operationRecords.Add(new OperationRecord(account, ip, OperrationType.Login));
                        if (actionDescriptor.ControllerName.ToLower() == "member")
                        {
                            //引擎登录
                            operationRecords.Add(new OperationRecord(account, ip, OperrationType.Engine));
                        }
                    }
                    else if (code == ResultCode.SpaFailed)
                    {
                        var requestModel = JsonConvert.DeserializeObject<loginInput>(requestBody);
                        account = requestModel.Account;
                        operationRecords.Add(new OperationRecord(account, ip, OperrationType.PwdError));
                    }
                    break;
                case "logout":
                    operationRecords.Add(new OperationRecord(account, ip, OperrationType.Logout));
                    break;
                case "index":
                    operationRecords.Add(new OperationRecord(account, ip, OperrationType.Manager));
                    break;
                case "resale":
                    operationRecords.Add(new OperationRecord(account, ip, OperrationType.ReSale));
                    break;
                default:
                    break;
            }
            return operationRecords;
        }

        private Encoding SelectCharacterEncoding(string contentType)
        {
            var mediaType = contentType == null ? new MediaType() : new MediaType(contentType);

            return mediaType.Encoding ?? Encoding.ASCII;
        }

        #endregion
    }
}
