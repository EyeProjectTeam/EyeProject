using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using EyeProtect.Fetch.Service.Repository;
using Volo.Abp.EntityFrameworkCore;

namespace EyeProtect;

[DependsOn(typeof(EyeProtectDomainModule))]
public class EyeProtectServiceModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var config = services.GetConfiguration();

        context.Services.AddAbpDbContext<EyeProtectDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseMySQL();
        });

    }
}
