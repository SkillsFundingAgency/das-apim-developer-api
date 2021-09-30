using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Apim.Developer.Data.Configuration
{
    public class SubscriberType : IEntityTypeConfiguration<Domain.Entities.SubscriberType>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.SubscriberType> builder)
        {
            builder.ToTable("SubscriberType");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("Id").HasColumnType("int").IsRequired();
            builder.Property(x => x.Name).HasColumnName("Name").HasColumnType("varchar").HasMaxLength(50).IsRequired();

            builder.HasIndex(x => x.Id).IsUnique();
        }
    }
}