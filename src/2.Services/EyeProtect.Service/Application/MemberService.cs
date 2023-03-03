using EyeProject.Core.Dto;
using EyeProtect.Core.Enums;
using EyeProtect.Core.Options;
using EyeProtect.Domain.Members;
using EyeProtect.Dtos;
using EyeProtect.Repository;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
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
using EyeProtect.Contract.Dtos;
using EyeProtect.Core.Cache;
using EyeProtect.Core;

namespace EyeProtect.Application
{
    /// <summary>
    /// 账号服务
    /// </summary>
    public class MemberService : ApplicationService, IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IOperationRecordRepository _operationRecordRepository;
        private readonly ICache<string> _cache;
        private readonly JwtOptions _jwtOptions;
        private readonly SupAdminOptions _supAdminOption;
        private readonly AccountLockOptions _accountLockOptions;

        /// <summary>
        /// ctor
        /// </summary>
        public MemberService(IMemberRepository memberRepository,
            IOptions<JwtOptions> jwtOptions,
            IOptions<SupAdminOptions> supAdminOption,
            IOptions<AccountLockOptions> accountLockOptions,
            IOperationRecordRepository operationRecordRepository,
            ICacheManager cacheManager)
        {
            _supAdminOption = supAdminOption.Value;
            _jwtOptions = jwtOptions.Value;
            _accountLockOptions = accountLockOptions.Value;
            _memberRepository = memberRepository;
            _operationRecordRepository = operationRecordRepository;
            _cache = cacheManager.GetCache<string>();
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
                if (await _cache.ExistsAsync(GetLockKey(input.Account)))
                {
                    return (ResultCode.CallLimited, "超出登录次数限制，账户已被锁定");
                }
                var member = await _memberRepository.FirstOrDefaultAsync(x => x.Account == input.Account);
                if (member == null)
                {
                    return (ResultCode.NoRecord, "用户名或密码不匹配，登录失败");
                }
                if (member.Password != input.Password)
                {
                    var lockCount = await LockAccountAsync(input.Account);
                    if (lockCount == 0)
                    {
                        return (ResultCode.SpaFailed, $"超出登录次数限制，账户被锁定");
                    }
                    return (ResultCode.SpaFailed, $"用户名或密码不匹配，登录失败，剩余重试次数：{lockCount}");
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
        /// <returns></returns>
        public async Task<Result> UpdateAccountType(long id, OperateAccountType operateAccountType)
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
            member.AccountType = AccountType.Sale;
            await _memberRepository.UpdateAsync(member);
            return Result.Ok();
        }

        /// <summary>
        /// 过期账号
        /// </summary>
        /// <returns></returns>
        public async Task<Result> ExpireAccount()
        {
            var startDate = DateTime.Now.AddYears(-1);
            var memberList = await (await _memberRepository.GetQueryableAsync()).Where(x => x.CreationTime <= startDate && x.AccountType == AccountType.Sale).ToListAsync();
            if (memberList.Any())
            {
                memberList.ForEach(x => { x.AccountType = AccountType.Expire; });
                await _memberRepository.UpdateManyAsync(memberList);
            }
            return Result.Ok();
        }

        /// <summary>
        /// 统计账号数据
        /// </summary>
        /// <returns></returns>
        public async Task<Result<StaticAccountDataOutput>> StaticAccountData()
        {
            var sale = await _memberRepository.CountAsync(x => x.AccountType == AccountType.Sale);
            var unSale = await _memberRepository.CountAsync(x => x.AccountType == AccountType.UnSale);
            var expire = await _memberRepository.CountAsync(x => x.AccountType == AccountType.Expire);

            var currDate = DateTime.Now;
            var startDate = new DateTime(currDate.Year, currDate.Month, 1);
            var visiteRecord = await (await _operationRecordRepository.GetQueryableAsync()).Where(x => x.CreationTime >= startDate && x.OperrationType == OperrationType.Engine).ToListAsync();
            var statitc = visiteRecord.GroupBy(x => x.CreationTime.Day).Select(x => new
            {
                Day = x.Key,
                Count = x.Count()
            }).ToList();
            var xAxis = new List<string>();
            for (var idate = startDate; idate <= startDate.AddMonths(1).AddDays(-1); idate = idate.AddDays(1))
            {
                xAxis.Add($"{idate.Month}月{idate.Day}日");

                if (!statitc.Any(x => x.Day == idate.Day))
                {
                    statitc.Add(new
                    {
                        Day = idate.Day,
                        Count = 0
                    });
                }
            }
            return Result.FromData(new StaticAccountDataOutput()
            {
                UnSale = unSale,
                Sale = sale,
                Expire = expire,
                EchartsData = new EchartsData
                {
                    XAxis = xAxis,
                    Data = statitc.OrderBy(x => x.Day).Select(x => x.Count).ToList(),
                }
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

        private string GetLockKey(string account)
        {
            return $"lockAccount:{account}";
        }

        private async Task<int> LockAccountAsync(string account)
        {
            //锁定范围起始时间
            var startDate = DateTime.Now.AddMinutes(_accountLockOptions.LockRange * -1);
            var lockCount = await _operationRecordRepository.CountAsync(x => x.CreationTime >= startDate && x.Account == account);
            if (lockCount > _accountLockOptions.LockNum)
            {
                if (!await _cache.ExistsAsync(GetLockKey(account)))
                {
                    await _cache.SetAsync<string>(GetLockKey(account), "1", TimeSpan.FromMinutes(_accountLockOptions.LockTime));
                }
                return 0;
            }
            if (lockCount == _accountLockOptions.LockNum)
            {
                await _cache.SetAsync<string>(GetLockKey(account), "1", TimeSpan.FromMinutes(_accountLockOptions.LockTime));
            }
            return _accountLockOptions.LockNum - lockCount;
        }

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
