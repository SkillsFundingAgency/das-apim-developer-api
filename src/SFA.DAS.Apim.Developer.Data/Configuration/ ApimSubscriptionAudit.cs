using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Apim.Developer.Data.Configuration
{
    public class ApimSubscriptionAudit : IEntityTypeConfiguration<Domain.Entities.ApimSubscriptionsAudit>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ApimSubscriptionsAudit> builder)
        {
            builder.ToTable("ApimSubscriptionAudit");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).HasColumnName("UserId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.ProductName).HasColumnName("ProductName").HasColumnType("varchar").HasMaxLength(250).IsRequired();
            builder.Property(x => x.Action).HasColumnName("Action").HasColumnType("varchar").HasMaxLength(50).IsRequired();
            builder.Property(x => x.Timestamp).HasColumnName("Timestamp").HasColumnType("datetime").IsRequired();

            builder.HasIndex(x => x.Id).IsUnique();
        }
    }
}