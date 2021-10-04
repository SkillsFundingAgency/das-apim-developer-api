using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Infrastructure.Api;

namespace SFA.DAS.Apim.Developer.Api.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services)
        {
            services.AddHttpClient<IAzureApimManagementService, AzureApimManagementService>();
            services.AddHttpClient<IAzureApimResourceService, AzureApimResourceService>();
            
            services.AddTransient<IAzureTokenService, AzureTokenService>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();
            services.AddSingleton(typeof(AzureServiceTokenProvider));
            
            services.AddSingleton(serviceProvider =>
            {
                var service = serviceProvider.GetService<IAzureApimResourceService>();
                var apimResourceId = service.GetResourceId().Result;
                var options = new ApimResourceConfiguration
                {
                    ApimResourceId = apimResourceId
                };
                
                return options;
            });
        }
    }
}