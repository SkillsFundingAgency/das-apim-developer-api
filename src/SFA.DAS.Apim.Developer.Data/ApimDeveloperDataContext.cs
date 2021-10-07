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
        DbSet<Domain.Entities.ApimUserType> ApimUserType { get; set; }
        DbSet<Domain.Entities.ApimUser> ApimUser { get; set; }
        DbSet<Domain.Entities.ApimAudit> ApimAudit { get; set; }

        int SaveChanges();
    }

    public partial class ApimDeveloperDataContext : DbContext, IApimDeveloperDataContext
    {
        private const string AzureResource = "https://database.windows.net/";
        
        private readonly ApimDeveloperApiConfiguration _configuration;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;
        private readonly EnvironmentConfiguration _environmentConfiguration;
        public DbSet<Domain.Entities.ApimUserType> ApimUserType { get; set; }
        public DbSet<Domain.Entities.ApimUser> ApimUser { get; set; }
        public DbSet<Domain.Entities.ApimAudit> ApimAudit { get; set; }

        public ApimDeveloperDataContext()
        {
        }

        public ApimDeveloperDataContext (DbContextOptions options) : base(options)
        {
            
        }
        public ApimDeveloperDataContext(IOptions<ApimDeveloperApiConfiguration> config, DbContextOptions options, AzureServiceTokenProvider azureServiceTokenProvider, EnvironmentConfiguration environmentConfiguration) :base(options)
        {
            _configuration = config.Value;
            _azureServiceTokenProvider = azureServiceTokenProvider;
            _environmentConfiguration = environmentConfiguration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            if (_configuration == null 
                || _environmentConfiguration.EnvironmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase)
                || _environmentConfiguration.EnvironmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
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
            modelBuilder.ApplyConfiguration(new ApimUserType());
            modelBuilder.ApplyConfiguration(new ApimUser());
            modelBuilder.ApplyConfiguration(new ApimAudit());

            base.OnModelCreating(modelBuilder);
        }
    }
}
