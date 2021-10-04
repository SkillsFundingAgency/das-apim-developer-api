using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Apim.Developer.Domain.Configuration;

namespace SFA.DAS.Apim.Developer.Api.AppStart
{
    public static class AddConfigurationOptionsExtension
    {
        public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ApimDeveloperApiConfiguration>(configuration.GetSection("ApimDeveloperApi"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ApimDeveloperApiConfiguration>>().Value);
            services.Configure<AzureActiveDirectoryConfiguration>(configuration.GetSection("AzureAd"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<AzureActiveDirectoryConfiguration>>().Value);
            services.Configure<AzureApimManagementConfiguration>(configuration.GetSection("AzureApimManagement"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<AzureApimManagementConfiguration>>().Value);
        }
    }
}