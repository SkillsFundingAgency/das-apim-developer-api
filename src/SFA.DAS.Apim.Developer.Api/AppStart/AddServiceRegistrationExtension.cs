using System;
using System.Net.Http;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Data.Repository;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Infrastructure.Api;

namespace SFA.DAS.Apim.Developer.Api.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services)
        {
            services.AddHttpClient<IAzureApimManagementService, AzureApimManagementService>()
                .AddPolicyHandler(HttpClientRetryPolicy());

            services.AddHttpClient<IAzureUserAuthenticationManagementService, AzureUserAuthenticationManagementService>()
                .AddPolicyHandler(HttpClientRetryPolicy());
            
            services.AddTransient<IAzureClientCredentialHelper, AzureClientCredentialHelper>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductService, ProductService>();
            
            services.AddTransient<IApimUserRepository, ApimUserRepository>();
            services.AddTransient<IApimAuditRepository, ApimAuditRepository>();
            services.AddTransient<IApimSubscriptionAuditRepository, ApimSubscriptionAuditRepository>();
        }

        private static IAsyncPolicy<HttpResponseMessage> HttpClientRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}