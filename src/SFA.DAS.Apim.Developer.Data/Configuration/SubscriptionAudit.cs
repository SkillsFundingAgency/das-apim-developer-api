using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Apim.Developer.Data.Configuration
{
    public class SubscriptionAudit : IEntityTypeConfiguration<Domain.Entities.SubscriptionAudit>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.SubscriptionAudit> builder)
        {
            builder.ToTable("SubscriptionAudit");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SubscriptionId).HasColumnName("SubscriptionId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.UserRef).HasColumnName("UserRef").HasColumnType("string").IsRequired();
            builder.Property(x => x.Action).HasColumnName("Action").HasColumnType("varchar").HasMaxLength(50).IsRequired();
            
            builder.HasOne(c => c.Subscription).WithMany(c => c.SubscriptionAudits)
                   .HasForeignKey(c => c.SubscriptionId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
