using System;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;

        public SubscriptionService(IAzureApimManagementService azureApimManagementService)
        {
            _azureApimManagementService = azureApimManagementService;
        }

        public async Task<Guid> CreateUserSubscription(Subscription subscription)
        {
            //TODO: 
            //await _azureApimManagementService.CreateSubscription("subscriptionId", "subscriberType", "internalUserRef", "apimUserId", "productId");

            return new Guid();
        }
    }
}