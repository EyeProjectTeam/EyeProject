using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SpiderManage.EntityFrameworkCore.Fetch;

public class SpiderManageDbContextFactory : IDesignTimeDbContextFactory<SpiderManageDbContext>
{
    public SpiderManageDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var connStr = configuration.GetConnectionString("Default");
        var builder = new DbContextOptionsBuilder<SpiderManageDbContext>()
            .UseMySql(connStr, ServerVersion.AutoDetect(connStr));

        return new SpiderManageDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
