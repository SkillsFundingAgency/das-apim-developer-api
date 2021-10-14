using System;
using System.Net.Http;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        public static void AddServiceRegistration(this IServiceCollection services)
        {
            services.AddHttpClient<IAzureApimManagementService, AzureApimManagementService>()
                .AddPolicyHandler(HttpClientRetryPolicy());
            services.AddHttpClient<IAzureApimResourceService, AzureApimResourceService>()
                .AddPolicyHandler(HttpClientRetryPolicy());

            services.AddTransient<IAzureTokenService, AzureTokenService>();
            services.AddTransient<ISubscriptionService, SubscriptionService>();
            services.AddTransient<IUserService, UserService>();
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


            services.AddTransient<IApimUserRepository, ApimUserRepository>();
            services.AddTransient<IApimAuditRepository, ApimAuditRepository>();
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