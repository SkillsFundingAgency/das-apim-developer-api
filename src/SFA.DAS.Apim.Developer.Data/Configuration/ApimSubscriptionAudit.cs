using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Apim.Developer.Data.Configuration
{
    public class ApimSubscriptionAudit : IEntityTypeConfiguration<Domain.Entities.ApimSubscriptionAudit>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ApimSubscriptionAudit> builder)
        {
            builder.ToTable("ApimSubscriptionAudit");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).HasColumnName("UserId").HasColumnType("varchar").HasMaxLength(250).IsRequired();
            builder.Property(x => x.ProductName).HasColumnName("ProductName").HasColumnType("varchar").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Action).HasColumnName("Action").HasColumnType("varchar(max)").IsRequired();
            builder.Property(x => x.Timestamp).HasColumnName("Timestamp").HasColumnType("datetime").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.ApimUserType).HasColumnName("ApimUserType").HasColumnType("smallint").IsRequired();

            builder.HasIndex(x => x.Id).IsUnique();
        }
    }
}