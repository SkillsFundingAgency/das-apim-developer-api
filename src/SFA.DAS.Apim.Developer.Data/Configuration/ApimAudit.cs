using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Apim.Developer.Data.Configuration
{
    public class ApimAudit : IEntityTypeConfiguration<Domain.Entities.ApimAudit>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ApimAudit> builder)
        {
            builder.ToTable("ApimAudit");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ApimUserId).HasColumnName("ApimUserId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.Action).HasColumnName("Action").HasColumnType("varchar").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Timestamp).HasColumnName("Timestamp").HasColumnType("datetime").IsRequired();
            
            builder.HasOne(c => c.ApimUser).WithMany(c => c.ApimAudits)
                   .HasForeignKey(c => c.ApimUserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
