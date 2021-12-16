using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using ApimUserType = SFA.DAS.Apim.Developer.Domain.Models.ApimUserType;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apim.Developer.Domain.Entities;
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
        private readonly IApimSubscriptionAuditRepository _apimSubscriptionAuditRepository;

        public SubscriptionService(
            IAzureApimManagementService azureApimManagementService,
            IApimSubscriptionAuditRepository apimSubscriptionAuditRepository,
            ILogger<SubscriptionService> logger)
        {
            _azureApimManagementService = azureApimManagementService;
            _logger = logger;
            _apimSubscriptionAuditRepository = apimSubscriptionAuditRepository;
        }
        
        public async Task<Subscription> CreateSubscription(string internalUserId,
            ApimUserType apimUserType, string productName)
        {
            var subscriptionId = $"{apimUserType}-{internalUserId}-{productName}";
            var createSubscriptionRequest = new CreateSubscriptionRequest(subscriptionId, productName);
            var apiResponse = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);
            await _apimSubscriptionAuditRepository.Insert(
                new ApimSubscriptionAudit
                {
                    Action = "new subscription",
                    ProductName = productName,
                    UserId = internalUserId,
                    ApimUserType = (short)apimUserType
                });

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

        public async Task RegenerateSubscriptionKeys(string internalUserId, ApimUserType apimUserType, string productName)
        {
            var subscriptionId = $"{apimUserType}-{internalUserId}-{productName}";
            
            var requestList = new List<IPostRequest>
            {
                new RegeneratePrimaryKeyRequest(subscriptionId),
                new RegenerateSecondaryKeyRequest(subscriptionId)
            };

            var taskList = requestList
                .Select(request => _azureApimManagementService.Post<string>(request))
                .ToList();

            await Task.WhenAll(taskList);

            var errorList = taskList
                .Where(task => !task.Result?.StatusCode.IsSuccessStatusCode() ?? false)
                .ToList();
            
            await _apimSubscriptionAuditRepository.Insert(
                new ApimSubscriptionAudit
                {
                    Action = "regenerate subscription",
                    ProductName = productName,
                    UserId = internalUserId,
                    ApimUserType = (short)apimUserType
                });
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