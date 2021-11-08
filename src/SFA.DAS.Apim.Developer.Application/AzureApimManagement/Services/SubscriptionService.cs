using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using ApimUserType = SFA.DAS.Apim.Developer.Domain.Models.ApimUserType;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apim.Developer.Domain.Extensions;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Responses;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses;

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
        
        public async Task<UserSubscription> CreateUserSubscription(string internalUserId,
            ApimUserType apimUserType, string productName, UserDetails userDetails)
        {
            var apimUserId = await _userService.CreateUser(internalUserId, userDetails, apimUserType);

            var subscriptionId = $"{apimUserType}-{internalUserId}";
            var createSubscriptionRequest = new CreateSubscriptionRequest(subscriptionId, apimUserId, productName);
            var apiResponse = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);
            
            if (!apiResponse.StatusCode.IsSuccessStatusCode())
            {
                _logger.LogError(apiResponse?.ErrorContent);
                throw new InvalidOperationException($"Response from create subscription for:[{subscriptionId}] was:[{apiResponse.StatusCode}]");
            }
            
            var sandboxSubscriptionId = $"{apimUserType}-{internalUserId}-sandbox";
            var createSandboxSubscriptionRequest = new CreateSubscriptionRequest(sandboxSubscriptionId, apimUserId, productName);
            var sandboxApiResponse = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSandboxSubscriptionRequest);

            if (!sandboxApiResponse.StatusCode.IsSuccessStatusCode())
            {
                _logger.LogError(sandboxApiResponse?.ErrorContent);
                throw new InvalidOperationException($"Response from create subscription for:[{sandboxSubscriptionId}] was:[{sandboxApiResponse.StatusCode}]");
            }

            return new UserSubscription
            {
                Id = apiResponse.Body.Id,
                Name = apiResponse.Body.Name,
                PrimaryKey = apiResponse.Body.Properties.PrimaryKey,
                SandboxPrimaryKey = sandboxApiResponse.Body.Properties.PrimaryKey
            };
        }

        public async Task<IEnumerable<UserSubscription>> GetUserSubscriptions(string internalUserId, ApimUserType apimUserType)
        {
            var apimSubscriptions =
                await _azureApimManagementService.Get<GetUserSubscriptionsResponse>(
                    new GetUserSubscriptionsRequest($"{apimUserType}-{internalUserId}"));

            var returnList = new List<UserSubscription>();
            foreach (var userSubscriptionItem in apimSubscriptions.Body.Value.Where(c=>c.Name.Contains($"{apimUserType}-{internalUserId}")))
            { 
                var subscriptionSecretsTask =
                    _azureApimManagementService.Post<GetUserSubscriptionSecretsResponse>(
                        new GetUserSubscriptionSecretsRequest(userSubscriptionItem.Name));

                var productTask =
                    _azureApimManagementService.Get<GetProductItem>(
                        new GetProductRequest(userSubscriptionItem.Properties.Scope.Split("/")[^1]));

                await Task.WhenAll(subscriptionSecretsTask, productTask);
                
                returnList.Add(new UserSubscription
                {
                    Id = userSubscriptionItem.Id,
                    Name = productTask.Result.Body.Name,
                    PrimaryKey = subscriptionSecretsTask.Result.Body.PrimaryKey
                });
            }

            return returnList;
        }
    }
}