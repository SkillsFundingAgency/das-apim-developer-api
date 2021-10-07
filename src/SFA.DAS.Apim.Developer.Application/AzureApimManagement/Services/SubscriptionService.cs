using System;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using ApimUserType = SFA.DAS.Apim.Developer.Domain.Models.ApimUserType;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        private readonly IApimUserRepository _apimUserRepository;
        private readonly IApimUserTypeRepository _apimUserTypeRepository;

        public SubscriptionService(IAzureApimManagementService azureApimManagementService, IApimUserRepository apimUserRepository, IApimUserTypeRepository apimUserTypeRepository)
        {
            _azureApimManagementService = azureApimManagementService;
            _apimUserRepository = apimUserRepository;
            _apimUserTypeRepository = apimUserTypeRepository;
        }

        public async Task<string> CreateUserSubscription(string internalUserId, string apimUserTypeName, string productName, Guid apimUserId)
        {
            var apimUserType = await _apimUserTypeRepository.Get(apimUserTypeName);
            var apimUser = await _apimUserRepository.GetByInternalIdAndType(internalUserId, apimUserType.Id);
            if (apimUser == null)
            {
                var newUser = new ApimUser
                {
                    ApimUserId = apimUserId, // TODO: should this be generated by SQL?
                    ApimUserTypeId = apimUserType.Id,
                    InternalUserId = internalUserId
                };
                
                var createApimUserTask = CreateApimUser(newUser);
                var apimUserTask = _apimUserRepository.Insert(newUser);
                await Task.WhenAll(createApimUserTask, apimUserTask);
                
                apimUser = apimUserTask.Result;
            }
            var subscriptionId = $"{apimUserType.Name}-{internalUserId}"; //TODO: think about this more, maybe just another guid?
            var createSubscriptionRequest = new CreateSubscriptionRequest(subscriptionId, apimUser.ApimUserId.ToString(), productName);
            var response = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);

            return subscriptionId;
        }

        private async Task<Guid> CreateApimUser(ApimUser apimUser)
        {
            var newUserRequest = new CreateUserRequest(apimUser.ApimUserId.ToString());
            var apimUserResponse = await _azureApimManagementService.Put<CreateUserResponse>(newUserRequest);
            return Guid.Parse(apimUserResponse.Body.Name);
        }

        public async Task<string> CreateUserSubscription(string internalUserId, ApimUserType apimUserType, string productName)
        {
            var apimUser = await _apimUserRepository.GetByInternalIdAndType(internalUserId, (int)apimUserType);

            if (apimUser == null)
            {
                var newUser = new ApimUser
                {
                    ApimUserId = Guid.NewGuid(), // TODO: should this be generated by SQL?
                    ApimUserTypeId = (int)apimUserType,
                    InternalUserId = internalUserId
                };
                
                var createApimUserTask = CreateApimUser(newUser);
                var apimUserTask = _apimUserRepository.Insert(newUser);
                await Task.WhenAll(createApimUserTask, apimUserTask);

                apimUser = apimUserTask.Result;
            }

            var createSubscriptionRequest = new CreateSubscriptionRequest($"{apimUserType}-{internalUserId}", apimUser.ApimUserId.ToString(), productName);
            var response = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);
            return response.Body.Properties.DisplayName;
        }
    }
}