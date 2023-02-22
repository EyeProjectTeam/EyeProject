using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using EyeProtect.Fetch.Service.Repository;
using Volo.Abp.EntityFrameworkCore;
using EyeProtect.Domain.Members;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EyeProtect.Dtos;

namespace EyeProtect;

[DependsOn(typeof(EyeProtectDomainModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class EyeProtectServiceModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var config = services.GetConfiguration();

        services.ConfigMapper(CreateDtoMappings);

        context.Services.AddAbpDbContext<EyeProtectDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(abpDbContextConfigurationContext =>
            {
                abpDbContextConfigurationContext.DbContextOptions.UseMySql(abpDbContextConfigurationContext.ConnectionString, new MySqlServerVersion("5.7"));
            });
        });
    }

    #region Method

    private void CreateDtoMappings(IMapperConfigurationExpression s)
    {
        s.CreateMap<Member, loginOuput>();
        s.CreateMap<Member, MemberPageListOutput>();
        s.CreateMap<Member, ExportMemberListInput>();
    }

    #endregion


}
