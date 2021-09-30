using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Apim.Developer.Data.Configuration
{
    public class Subscription : IEntityTypeConfiguration<Domain.Entities.Subscription>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Subscription> builder)
        {
            builder.ToTable("Subscription");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("Id").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.ExternalSubscriptionId).HasColumnName("ExternalSubscriptionId").HasColumnType("int").IsRequired();
            builder.Property(x => x.ExternalSubscriberId).HasColumnName("ExternalSubscriberId").HasColumnType("varchar").HasMaxLength(50).IsRequired();
            builder.Property(x => x.SubscriberTypeId).HasColumnName("SubscriberTypeId").HasColumnType("int").HasMaxLength(50).IsRequired();
            
            builder.HasIndex(x => x.Id).IsUnique();

            builder.HasOne(c => c.SubscriberType).WithMany(c => c.Subscriptions)
                   .HasForeignKey(c => c.SubscriberTypeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}