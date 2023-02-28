using EyeProject.Core.Dto;
using EyeProtect.Core.Enums;
using EyeProtect.Core.Options;
using EyeProtect.Domain.Members;
using EyeProtect.Dtos;
using EyeProtect.Repository;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mutone.Core.Utils.Excel;
using Mutone.Core.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Claims;
using Microsoft.EntityFrameworkCore;
using EyeProtect.Core.Utils;
using EyeProtect.Repository.Impl;
using System.Diagnostics.Metrics;
using EyeProtect.Contract.Dtos;
using Microsoft.Extensions.FileProviders;

namespace EyeProtect.Application
{
    /// <summary>
    /// 账号服务
    /// </summary>
    public class MemberService : ApplicationService, IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly JwtOptions _jwtOptions;
        private readonly SupAdminOptions _supAdminOption;

        /// <summary>
        /// ctor
        /// </summary>
        public MemberService(IMemberRepository memberRepository,
            IOptions<JwtOptions> jwtOptions,
            IOptions<SupAdminOptions> supAdminOption)
        {
            _supAdminOption = supAdminOption.Value;
            _jwtOptions = jwtOptions.Value;
            _memberRepository = memberRepository;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<loginOuput>> LoginAsync(loginInput input)
        {
            var loginOutput = new loginOuput();
            if (input.Account == _supAdminOption.Name)
            {
                #region 超级管理员登录

                if (input.Password != _supAdminOption.Pwd)
                {
                    return (ResultCode.Fail, "用户名或密码不匹配，登录失败");
                }
                loginOutput.Role = _supAdminOption.Name;
                loginOutput.Name = _supAdminOption.Name;
                loginOutput.Id = 123456;
                loginOutput.Account = _supAdminOption.Name;

                #endregion
            }
            else
            {
                var member = await _memberRepository.FirstOrDefaultAsync(x => x.Account == input.Account && x.Password == input.Password);
                if (member == null)
                {
                    return (ResultCode.Fail, "用户名或密码不匹配，登录失败");
                }
                loginOutput = member.MapTo<loginOuput>();
                loginOutput.Role = member.IsAdmin ? "Admin" : "Member";
            }
            var token = GenerateJwt(loginOutput);
            loginOutput.Token = token;
            loginOutput.ExpirationTime = _jwtOptions.ExpirationTime;
            return loginOutput;
        }

        /// <summary>
        /// 生成账号
        /// </summary>
        /// <returns></returns>
        public async Task<Result> MakeAccount(int number)
        {
            var memberList = new List<Member>();
            for (int i = 0; i < number; i++)
            {
                memberList.Add(MakeMember(false));
            }
            //是否存在管理员
            var admins = await _memberRepository.GetListAsync(x => x.IsAdmin);
            if (!admins.Any())
            {
                memberList.Add(MakeMember(true));
            }
            await _memberRepository.InsertManyAsync(memberList);
            return Result.Ok();
        }

        /// <summary>
        /// 账号列表
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<PageResult<MemberPageListOutput>> GetMemberPageList(MemberPageListInput input)
        {
            var query = await GetMemberQueryAsync(input);
            var result = await query.ToPageResultAsync<Member, MemberPageListOutput>(input);
            return result;
        }

        /// <summary>
        /// 更新账号状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operateAccountType">操作类型</param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public async Task<Result> UpdateAccountType(long id, OperateAccountType operateAccountType, AccountType accountType)
        {
            var member = await _memberRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (member == null)
            {
                return (ResultCode.NoRecord, "未查询到账号信息");
            }
            if (operateAccountType == OperateAccountType.ReSale && member.AccountType != AccountType.Expire)
            {
                return (ResultCode.InvalidData, "只有未过期的账号才可续期");
            }
            member.AccountType = accountType;
            await _memberRepository.UpdateAsync(member);
            return Result.Ok();
        }

        /// <summary>
        /// 统计账号数据
        /// </summary>
        /// <returns></returns>
        public async Task<Result<StaticAccountDataOutput>> StaticAccountData()
        {
            var used = await _memberRepository.CountAsync(x => x.AccountType == AccountType.Used);
            var unUse = await _memberRepository.CountAsync(x => x.AccountType == AccountType.UnUse);
            var unSale = await _memberRepository.CountAsync(x => x.AccountType == AccountType.UnSale);
            var expire = await _memberRepository.CountAsync(x => x.AccountType == AccountType.Expire);

            return Result.FromData(new StaticAccountDataOutput()
            {
                UnUse = unUse,
                UnSale = unSale,
                Used = used,
                Expire = expire
            });
        }

        /// <summary>
        /// 账号列表导出
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExportMemberList(MemberPageListInput input)
        {
            var fileName = "账号列表";
            //var exportInput = await (await GetMemberQueryAsync(input)).ToPageResultAsync<Member, ExportMemberListInput>(input);
            var exportInput = (await (await _memberRepository.GetQueryableAsync()).Where(x => !x.IsAdmin).ToListAsync()).MapTo<List<ExportMemberListInput>>();
            var excelBytes = ExcelHelper.WriteExcel(new ExcelWorksheetDto<ExportMemberListInput>()
            {
                WorksheetName = fileName,
                SerialNumberColumnIndex = 1,
                Data = exportInput,
                Heads = new List<ExcelHeadDto>() { new ExcelHeadDto() { Name = fileName, SpanCells = new[] { 1, 1, 1, 18 } }, }
            });
            return new FileContentResult(excelBytes, "application/octet-stream")
            {
                FileDownloadName = $"{fileName}{Clock.Now:yyyyMMdd}.xlsx"
            };
        }

        #region Method

        private Member MakeMember(bool isAdmin)
        {
            var account = Guid.NewGuid().ToString("N");
            var member = new Member()
            {
                Account = account.Substring(0, 12),
                Password = account.Substring(account.Length - 6),
                IsAdmin = isAdmin,
                AccountType = AccountType.UnSale
            };
            return member;
        }

        private async Task<IQueryable<Member>> GetMemberQueryAsync(MemberPageListInput input)
        {
            var query = await _memberRepository.GetQueryableAsync();
            if (input.AccountType.HasValue)
            {
                query = query.Where(x => x.AccountType == input.AccountType);
            }
            query = query.Where(x => !x.IsAdmin);
            return query;
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private string GenerateJwt(loginOuput member)
        {
            var dateNow = Clock.Now;
            var expirationTime = dateNow.AddHours(_jwtOptions.ExpirationTime);
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecurityKey);

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Audience, _jwtOptions.Audience),
                new Claim(JwtClaimTypes.Issuer, _jwtOptions.Issuer),
                new Claim(AbpClaimTypes.UserId, member.Account??string.Empty),
                new Claim(JwtClaimTypes.Id, member.Id.ToString()),
                new Claim(AbpClaimTypes.UserName, member.Name??string.Empty),
                new Claim(AbpClaimTypes.Email, member.Email??string.Empty),
                new Claim(AbpClaimTypes.Role,member.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime, // token 过期时间
                NotBefore = dateNow,    // token 签发时间
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token); ;
        }

        #endregion
    }
}
