using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.Apim.Developer.Data.Configuration
{
    public class ApimUser : IEntityTypeConfiguration<Domain.Entities.ApimUser>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.ApimUser> builder)
        {
            builder.ToTable("ApimUser");
            builder.HasKey(x => x.ApimUserId);

            builder.Property(x => x.ApimUserId).HasColumnName("ApimUserId").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.InternalUserId).HasColumnName("InternalUserId").HasColumnType("varchar").HasMaxLength(50).IsRequired();
            builder.Property(x => x.ApimUserTypeId).HasColumnName("ApimUserTypeId").HasColumnType("smallint").IsRequired();
            
            builder.HasIndex(x => x.ApimUserId).IsUnique();
        }
    }
}