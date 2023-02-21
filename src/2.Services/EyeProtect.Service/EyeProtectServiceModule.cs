using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using EyeProtect.Fetch.Service.Repository;
using Volo.Abp.EntityFrameworkCore;
using EyeProtect.Repository.Impl;
using EyeProtect.Domain.Members;
using EyeProtect.Members;
using EyeProtect.Repository;
using Microsoft.Extensions.Configuration;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;

namespace EyeProtect;

[DependsOn(typeof(EyeProtectDomainModule))]
[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
public class EyeProtectServiceModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var config = services.GetConfiguration();

        context.Services.AddAbpDbContext<EyeProtectDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
            //options.AddRepository<Member, MemberRepository>();
            //options.AddRepository<OperationRecord, OperationRecordRepository>();
        });

        Configure<AbpDbContextOptions>(options =>
        {
            var connectionString = config.GetConnectionString("Default");
            options.Configure(abpDbContextConfigurationContext =>
            {
                abpDbContextConfigurationContext.DbContextOptions.UseMySql(connectionString, new MySqlServerVersion("5.7"));
            });
        });

    }
}
