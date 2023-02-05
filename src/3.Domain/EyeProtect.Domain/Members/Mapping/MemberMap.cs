using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EyeProtect.Core.Repository;
using System;
using EyeProtect.Domain.Members;

namespace EyeProtect.Domain.Mapping
{
    public class MemberMap : EntityMap<Member>
    {
        public override void Configure(EntityTypeBuilder<Member> b)
        {
            base.Configure(b);

            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd();


        }
    }
}
