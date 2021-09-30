using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apim.Developer.Data.Configuration;

namespace SFA.DAS.Apim.Developer.Data
{
    public interface IApimDeveloperDataContext 
    {
        DbSet<Domain.Entities.SubscriberType> SubscriberType { get; set; }
        DbSet<Domain.Entities.Subscription> Subscription { get; set; }
        DbSet<Domain.Entities.SubscriptionAudit> SubscriptionAudit { get; set; }

        int SaveChanges();
    }

    public partial class ApimDeveloperDataContext : DbContext, IApimDeveloperDataContext
    {
        public DbSet<Domain.Entities.SubscriberType> SubscriberType { get; set; }
        public DbSet<Domain.Entities.Subscription> Subscription { get; set; }
        public DbSet<Domain.Entities.SubscriptionAudit> SubscriptionAudit { get; set; }

        public ApimDeveloperDataContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public ApimDeveloperDataContext(DbContextOptions options) :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SubscriberType());
            modelBuilder.ApplyConfiguration(new Subscription());
            modelBuilder.ApplyConfiguration(new SubscriptionAudit());

            base.OnModelCreating(modelBuilder);
        }
    }
}
