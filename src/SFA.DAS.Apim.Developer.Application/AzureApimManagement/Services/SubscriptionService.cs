using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using ApimUserType = SFA.DAS.Apim.Developer.Domain.Models.ApimUserType;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        private readonly IApimUserRepository _apimUserRepository;

        public SubscriptionService(IAzureApimManagementService azureApimManagementService, IApimUserRepository apimUserRepository)
        {
            _azureApimManagementService = azureApimManagementService;
            _apimUserRepository = apimUserRepository;
        }
        public async Task<CreateSubscriptionResponse> CreateUserSubscription(string internalUserId, ApimUserType apimUserType, string productName, UserDetails userDetails)
        {
            var apimUser = await _apimUserRepository.GetByInternalIdAndType(internalUserId, (int)apimUserType);

            var apimUserId = "";
            
            if (apimUser == null)
            {
                var createApimUserTask = await CreateApimUser(Guid.NewGuid().ToString(), userDetails);
                
                var newUser = new ApimUser
                {
                    ApimUserTypeId = (int)apimUserType,
                    InternalUserId = internalUserId,
                    ApimUserId = createApimUserTask 
                };
                
                await _apimUserRepository.Insert(newUser);
                
                apimUserId = createApimUserTask;
            }
            else
            {
                apimUserId = apimUser.ApimUserId;    
            }
            
            
            var createSubscriptionRequest = new CreateSubscriptionRequest($"{apimUserType}-{internalUserId}", apimUserId, productName);
            var response = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);
            return response.Body;
        }

        private async Task<string> CreateApimUser(string apimUserId, UserDetails userDetails)
        {
            var newUserRequest = new CreateUserRequest(apimUserId, userDetails);
            var apimUserResponse = await _azureApimManagementService.Put<CreateUserResponse>(newUserRequest);

            if (apimUserResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var getUserRequest = new GetApimUserRequest(userDetails.EmailAddress);
                var getUserResponse = await _azureApimManagementService.Get<ApimUserResponse>(getUserRequest);

                return getUserResponse.Body.Properties.First().Name;
            }
            
            return apimUserResponse.Body.Name;
        }
    }
}