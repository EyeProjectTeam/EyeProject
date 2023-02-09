using EyeProtect.Members;
using Volo.Abp.Domain.Repositories;

namespace EyeProtect.Repository
{
    public interface IOperationRecordRepository : IRepository<OperationRecord, long>
    {
    }
}
