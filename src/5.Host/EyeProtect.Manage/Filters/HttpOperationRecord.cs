using EyeProtect.Core.Utils;
using EyeProtect.Members;
using EyeProtect.Repository;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
            var logger = _loggerFactory.CreateLogger("OperationRecord");
            try
            {
                var claims = _currentUser.GetAllClaims();
                if (claims != null && claims.Any())
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString();
                    var operationRecord = new OperationRecord()
                    {
                        MemberId = long.Parse(claims.GetClaimValue(JwtClaimTypes.Id)),
                        Account = claims.GetClaimValue(AbpClaimTypes.UserId),
                        MemberName = _currentUser.UserName,
                        Ip = ip,
                    };
                    await _operationRecordRepository.InsertAsync(operationRecord);
                }
                await next(context);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                await next(context);
            }

        }
    }
}
