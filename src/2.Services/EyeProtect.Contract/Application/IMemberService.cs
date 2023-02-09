using EyeProject.Core.Dto;
using EyeProtect.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EyeProtect.Application
{
    public interface IMemberService : IApplicationService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Task<Result<loginOuput>> LoginAsync(loginInput input);
    }
}
