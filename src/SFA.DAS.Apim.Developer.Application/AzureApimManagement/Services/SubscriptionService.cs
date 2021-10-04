
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        private readonly IOptions<ApimResourceConfiguration> _apimResourceConfig;

        public SubscriptionService(IAzureApimManagementService azureApimManagementService)
        {
            _azureApimManagementService = azureApimManagementService;
        }

        public async Task<Guid> CreateUserSubscription(Subscription subscription)
        {
            throw new NotImplementedException();
            //TODO: pass real values
            var createSubscriptionRequest = new CreateSubscriptionRequest("subscriptionId", "subscriberType", "internalUserRef", "apimUserId", "productId");
            var response = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);

            //TODO: return something
            return new Guid();
        }
    }
}