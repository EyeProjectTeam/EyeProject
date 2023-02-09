using EyeProtect.Domain.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EyeProtect.Repository
{
    public interface IMemberRepository : IRepository<Member, long>
    {
    }
}
