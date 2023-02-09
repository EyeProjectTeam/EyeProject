using EyeProtect.Fetch.Service.Repository;
using EyeProtect.Members;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EyeProtect.Repository.Impl
{
    public class OperationRecordRepository : EfCoreRepository<EyeProtectDbContext, OperationRecord, long>, IOperationRecordRepository
    {
        public OperationRecordRepository(IDbContextProvider<EyeProtectDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
