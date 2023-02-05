using Microsoft.EntityFrameworkCore;
using System.Linq;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using EyeProtect.Domain.Members;
using EyeProtect.Members;

namespace EyeProtect.Fetch.Service.Repository
{

    [ConnectionStringName("Default")]
    public class EyeProtectDbContext : AbpDbContext<EyeProtectDbContext>
    {
        #region DbSet

        public DbSet<Member> Members { get; set; }

        public DbSet<OperationRecord> OperationRecords { get; set; }

        #endregion

        public EyeProtectDbContext(DbContextOptions<EyeProtectDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 全局添加map
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        }
    }
}