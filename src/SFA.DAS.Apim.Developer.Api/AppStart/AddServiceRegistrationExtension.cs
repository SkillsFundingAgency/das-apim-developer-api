using System;
using System.Net.Http;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Data.Repository;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Infrastructure.Api;

namespace SFA.DAS.Apim.Developer.Api.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        private const int MaxRetries = 2;
        private static readonly TimeSpan NetworkTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(100);
        public static void AddServiceRegistration(this IServiceCollection services)
        {
            services.AddHttpClient<IAzureApimManagementService, AzureApimManagementService>()
                .AddPolicyHandler(HttpClientRetryPolicy());

            services.AddHttpClient<IAzureUserAuthenticationManagementService, AzureUserAuthenticationManagementService>()
                .AddPolicyHandler(HttpClientRetryPolicy());
            
            services.AddTransient<IAzureTokenService, AzureTokenService>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddSingleton(new ChainedTokenCredential(
                new ManagedIdentityCredential(options: new TokenCredentialOptions
                {
                    Retry = { NetworkTimeout = NetworkTimeout, MaxRetries = MaxRetries, Delay = Delay }
                }),
                new AzureCliCredential(options: new AzureCliCredentialOptions
                {
                    Retry = { NetworkTimeout = NetworkTimeout, MaxRetries = MaxRetries, Delay = Delay }
                }),
                new VisualStudioCredential(options: new VisualStudioCredentialOptions
                {
                    Retry = { NetworkTimeout = NetworkTimeout, MaxRetries = MaxRetries, Delay = Delay }
                }),
                new VisualStudioCodeCredential(options: new VisualStudioCodeCredentialOptions()
                {
                    Retry = { NetworkTimeout = NetworkTimeout, MaxRetries = MaxRetries, Delay = Delay }
                }))
            );

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