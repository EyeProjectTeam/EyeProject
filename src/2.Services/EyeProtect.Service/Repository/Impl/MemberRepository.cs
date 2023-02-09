﻿using EyeProtect.Domain.Members;
using EyeProtect.Fetch.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EyeProtect.Repository.Impl
{
    public class MemberRepository : EfCoreRepository<EyeProtectDbContext, Member, long>, IMemberRepository
    {
        public MemberRepository(IDbContextProvider<EyeProtectDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
