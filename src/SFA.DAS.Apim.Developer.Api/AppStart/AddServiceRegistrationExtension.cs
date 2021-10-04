using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
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
        }
    }
}