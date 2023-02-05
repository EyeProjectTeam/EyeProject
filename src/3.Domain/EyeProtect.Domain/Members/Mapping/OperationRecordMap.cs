using EyeProtect.Core.Repository;
using EyeProtect.Domain.Members;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeProtect.Members.Mapping
{
    public class OperationRecordMap : EntityMap<OperationRecord>
    {
        public override void Configure(EntityTypeBuilder<OperationRecord> b)
        {
            base.Configure(b);

            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedOnAdd();

            b.HasIndex(x => x.MemberId);
            b.HasIndex(x => x.MemberName);
            b.HasIndex(x => x.OperrationType);
            b.HasIndex(x => new { x.MemberId, x.OperrationType });
        }
    }
}
