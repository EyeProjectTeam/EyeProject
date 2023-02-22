using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using EyeProtect.Fetch.Service.Repository;

namespace EyeProtect.EntityFrameworkCore.Fetch;

public class EyeProtectDbContextFactory : IDesignTimeDbContextFactory<EyeProtectDbContext>
{
    public EyeProtectDbContext CreateDbContext(string[] args)
    {
        var connStr = "Server=127.0.0.1;port=3306;database=EyeProtect;user id=root;password=106617;minimumpoolsize=10;maximumpoolsize=50";
        var builder = new DbContextOptionsBuilder<EyeProtectDbContext>()
            .UseMySql(connStr, ServerVersion.AutoDetect(connStr));

        return new EyeProtectDbContext(builder.Options);
    }

}
