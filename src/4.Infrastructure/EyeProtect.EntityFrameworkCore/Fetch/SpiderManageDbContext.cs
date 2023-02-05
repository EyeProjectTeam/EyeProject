using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace SpiderManage.EntityFrameworkCore.Fetch
{

    [ConnectionStringName("Default")]
    public class SpiderManageDbContext : AbpDbContext<SpiderManageDbContext>
    {
        #region DbSet

        #endregion

        public SpiderManageDbContext(DbContextOptions<SpiderManageDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}