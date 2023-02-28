using EyeProject.Core.Dto;
using EyeProtect.Contract.Dtos;
using EyeProtect.Core.Enums;
using EyeProtect.Dtos;
using Microsoft.AspNetCore.Mvc;
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
        Task<Result<loginOuput>> LoginAsync(loginInput input);

        Task<Result> MakeAccount(int number);

        Task<PageResult<MemberPageListOutput>> GetMemberPageList(MemberPageListInput input);

        Task<Result> UpdateAccountType(long id, OperateAccountType operateAccountType);

        Task<IActionResult> ExportMemberList(MemberPageListInput input);

        Task<Result<StaticAccountDataOutput>> StaticAccountData();
    }
}
