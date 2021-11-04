using System;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using ApimUserType = SFA.DAS.Apim.Developer.Domain.Models.ApimUserType;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apim.Developer.Domain.Extensions;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        private readonly IUserService _userService;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(
            IAzureApimManagementService azureApimManagementService, 
            IUserService userService,
            ILogger<SubscriptionService> logger)
        {
            _azureApimManagementService = azureApimManagementService;
            _userService = userService;
            _logger = logger;
        }
        
        public async Task<Subscription> CreateSubscription(string internalUserId,
            ApimUserType apimUserType, string productName)
        {
            var subscriptionId = $"{apimUserType}-{internalUserId}";
            var createSubscriptionRequest = new CreateSubscriptionRequest(subscriptionId, productName);
            var apiResponse = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);
            
            if (!apiResponse.StatusCode.IsSuccessStatusCode())
            {
                _logger.LogError(apiResponse?.ErrorContent);
                throw new InvalidOperationException($"Response from create subscription for:[{subscriptionId}] was:[{apiResponse.StatusCode}]");
            }
            
            var sandboxSubscriptionId = $"{apimUserType}-{internalUserId}-sandbox";
            var createSandboxSubscriptionRequest = new CreateSubscriptionRequest(sandboxSubscriptionId, productName);
            var sandboxApiResponse = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSandboxSubscriptionRequest);

            if (!sandboxApiResponse.StatusCode.IsSuccessStatusCode())
            {
                _logger.LogError(sandboxApiResponse?.ErrorContent);
                throw new InvalidOperationException($"Response from create subscription for:[{sandboxSubscriptionId}] was:[{sandboxApiResponse.StatusCode}]");
            }

            return new Subscription
            {
                Id = apiResponse.Body.Id,
                Name = apiResponse.Body.Name,
                PrimaryKey = apiResponse.Body.Properties.PrimaryKey,
                SandboxPrimaryKey = sandboxApiResponse.Body.Properties.PrimaryKey
            };
        }
    }
}