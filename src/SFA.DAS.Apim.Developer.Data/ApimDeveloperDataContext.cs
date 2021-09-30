using System;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Developer.Data.Configuration;
using SFA.DAS.Apim.Developer.Domain.Configuration;

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
        private const string AzureResource = "https://database.windows.net/";
        
        private readonly ApimDeveloperApiConfiguration _configuration;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;
        public DbSet<Domain.Entities.SubscriberType> SubscriberType { get; set; }
        public DbSet<Domain.Entities.Subscription> Subscription { get; set; }
        public DbSet<Domain.Entities.SubscriptionAudit> SubscriptionAudit { get; set; }

        public ApimDeveloperDataContext()
        {
        }

        public ApimDeveloperDataContext (DbContextOptions options) : base(options)
        {
            
        }
        public ApimDeveloperDataContext(IOptions<ApimDeveloperApiConfiguration> config, DbContextOptions options, AzureServiceTokenProvider azureServiceTokenProvider) :base(options)
        {
            _configuration = config.Value;
            _azureServiceTokenProvider = azureServiceTokenProvider;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            if (_configuration == null || _azureServiceTokenProvider == null)
            {
                return;
            }
            
            var connection = new SqlConnection
            {
                ConnectionString = _configuration.ConnectionString,
                AccessToken = _azureServiceTokenProvider.GetAccessTokenAsync(AzureResource).Result,
            };
            
            optionsBuilder.UseSqlServer(connection,options=>
                options.EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(20),
                    null
                ));
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
