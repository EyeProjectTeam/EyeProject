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

namespace EyeProtect.Application
{
    /// <summary>
    /// 账号服务
    /// </summary>
    public class MemberService : ApplicationService, IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly JwtOptions _jwtOptions;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// ctor
        /// </summary>
        public MemberService(IMemberRepository memberRepository,
            IOptionsSnapshot<JwtOptions> jwtOptions,
            IConfiguration configuration)
        {
            _configuration = configuration;
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
            var md5Pwd = input.Password.ToMd5();
            var member = await _memberRepository.FirstOrDefaultAsync(x => x.Account == input.Account && x.Password == md5Pwd);
            if (member == null)
            {
                return (ResultCode.Fail, "用户名或密码不匹配，登录失败");
            }
            var token = GenerateJwt(member);
            var loginOutput = ObjectMapper.Map<Member, loginOuput>(member);
            loginOutput.Token = token;
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
                var account = Guid.NewGuid().ToString("N");
                var member = new Member()
                {
                    Account = account,
                    Password = account.Substring(account.Length - 6),
                    IsAdmin = false,
                    AccountType = AccountType.UnSale
                };
                memberList.Add(member);
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
            var result = await query.PageAsync<Member, MemberPageListOutput>(input);
            return result;
        }

        /// <summary>
        /// 更新账号状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public async Task<Result> UpdateAccountType(long id, AccountType accountType)
        {
            var member = await _memberRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (member == null)
            {
                return (ResultCode.NoRecord, "未查询到账号信息");
            }
            if (accountType == AccountType.Used && member.AccountType != AccountType.Expire)
            {
                return (ResultCode.InvalidData, "只有未过期的账号才可续期");
            }
            member.AccountType = accountType;
            await _memberRepository.UpdateAsync(member);
            return Result.Ok();
        }

        /// <summary>
        /// 账号列表导出
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExportMemberList(MemberPageListInput input)
        {
            var fileName = "账号列表";
            var result = (await GetMemberQueryAsync(input)).ToList();
            var exportInput = ObjectMapper.Map<List<Member>, List<ExportMemberListInput>>(result);
            var excelBytes = ExcelHelper.WriteExcel(new ExcelWorksheetDto<ExportMemberListInput>()
            {
                WorksheetName = fileName,
                SerialNumberColumnIndex = 1,
                Data = exportInput,
                Heads = new List<ExcelHeadDto>() { new ExcelHeadDto() { Name = fileName }, }
            });
            return new FileContentResult(excelBytes, "application/octet-stream")
            {
                FileDownloadName = $"{fileName}{Clock.Now:yyyyMMdd}.xlsx"
            };
        }

        #region Method

        private async Task<IQueryable<Member>> GetMemberQueryAsync(MemberPageListInput input)
        {
            var query = await _memberRepository.GetQueryableAsync();
            if (!input.Account.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.Account == input.Account);
            }
            if (input.AccountType.HasValue)
            {
                query = query.Where(x => x.AccountType == input.AccountType);
            }
            return query;
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private string GenerateJwt(Member member)
        {
            var isSupAdmin = _configuration["SuperAdmin:Name"] == member.Name;
            var dateNow = Clock.Now;
            var expirationTime = dateNow.AddHours(_jwtOptions.ExpirationTime);
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecurityKey);

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Audience, _jwtOptions.Audience),
                new Claim(JwtClaimTypes.Issuer, _jwtOptions.Issuer),
                new Claim(AbpClaimTypes.UserId, member.Account),
                new Claim(JwtClaimTypes.Id, member.Id.ToString()),
                new Claim(AbpClaimTypes.UserName, member.Name),
                new Claim(AbpClaimTypes.Email, member.Email),
                new Claim(AbpClaimTypes.Role,isSupAdmin?"SuperAdmin": member.IsAdmin?"Admin":"Member")
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
