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
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(
            IAzureApimManagementService azureApimManagementService,
            ILogger<SubscriptionService> logger)
        {
            _azureApimManagementService = azureApimManagementService;
            _logger = logger;
        }
        
        public async Task<Subscription> CreateSubscription(string internalUserId,
            ApimUserType apimUserType, string productName)
        {
            var subscriptionId = $"{apimUserType}-{internalUserId}-{productName}";
            var createSubscriptionRequest = new CreateSubscriptionRequest(subscriptionId, productName);
            var apiResponse = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);
            
            if (!apiResponse.StatusCode.IsSuccessStatusCode())
            {
                _logger.LogError(apiResponse?.ErrorContent);
                throw new InvalidOperationException($"Response from create subscription for:[{subscriptionId}] was:[{apiResponse.StatusCode}]");
            }
            
            return new Subscription
            {
                Id = apiResponse.Body.Id,
                Name = apiResponse.Body.Name,
                PrimaryKey = apiResponse.Body.Properties.PrimaryKey
            };
        }

        public async Task RegenerateSubscription(string internalUserId, ApimUserType apimUserType)
        {
            var subscriptionId = $"{apimUserType}-{internalUserId}";
            var sandboxSubscriptionId = $"{apimUserType}-{internalUserId}-sandbox";

            var requestList = new List<IPostRequest>
            {
                new RegeneratePrimaryKeyRequest(subscriptionId),
                new RegeneratePrimaryKeyRequest(sandboxSubscriptionId),
                new RegenerateSecondaryKeyRequest(subscriptionId),
                new RegenerateSecondaryKeyRequest(sandboxSubscriptionId)
            };

            var taskList = requestList
                .Select(request => _azureApimManagementService.Post<object>(request))
                .ToList();

            await Task.WhenAll(taskList);

            var errorList = taskList
                .Where(task => !task.Result.StatusCode.IsSuccessStatusCode())
                .ToList();

            if (errorList.Count == 0)
            {
                return;
            }

            var exceptionList = new List<Exception>();
            for (var i = 0; i < taskList.Count; i++)
            {
                var apiResponse = taskList[i].Result;
                if (!apiResponse.StatusCode.IsSuccessStatusCode())
                {
                    _logger.LogError(apiResponse?.ErrorContent);
                    exceptionList.Add(new ApplicationException(
                        $"Response from regenerate key for:[{requestList[i].PostUrl}] was:[{apiResponse.StatusCode}]"));
                }
            }
            throw new AggregateException(exceptionList);
        }

        public async Task<IEnumerable<Subscription>> GetUserSubscriptions(string internalUserId, ApimUserType apimUserType)
        {
            var apimSubscriptions =
                await _azureApimManagementService.Get<GetSubscriptionsResponse>(
                    new GetUserSubscriptionsRequest($"{apimUserType}-{internalUserId}"));

            var returnList = new List<Subscription>();
            foreach (var userSubscriptionItem in apimSubscriptions.Body.Value.Where(c=>c.Name.Contains($"{apimUserType}-{internalUserId}")))
            { 
                var subscriptionSecretsTask =
                    _azureApimManagementService.Post<GetUserSubscriptionSecretsResponse>(
                        new GetUserSubscriptionSecretsRequest(userSubscriptionItem.Name));

                var productTask =
                    _azureApimManagementService.Get<GetProductItem>(
                        new GetProductRequest(userSubscriptionItem.Properties.Scope.Split("/")[^1]));

                await Task.WhenAll(subscriptionSecretsTask, productTask);
                
                returnList.Add(new Subscription
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