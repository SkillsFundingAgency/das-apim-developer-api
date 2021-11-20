using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        private readonly IApimUserRepository _apimUserRepository;

        public UserService(IAzureApimManagementService azureApimManagementService, IApimUserRepository apimUserRepository)
        {
            _azureApimManagementService = azureApimManagementService;
            _apimUserRepository = apimUserRepository;
        }
        
        public async Task<string> CreateUser(string internalUserId, UserDetails userDetails, ApimUserType apimUserType)
        {
            var apimUser = await _apimUserRepository.GetByInternalIdAndType(internalUserId, (int)apimUserType);
            
            if (apimUser != null)
            {
                return apimUser.ApimUserId;
            }
            
            var createApimUserTask = await CreateApimUser(Guid.NewGuid().ToString(), userDetails);

            await _apimUserRepository.Insert(new ApimUser
            {
                ApimUserTypeId = (int)apimUserType,
                InternalUserId = internalUserId,
                ApimUserId = createApimUserTask 
            });
                
            return createApimUserTask;

        }

        public async Task<ApimUser> GetUser(string internalUserId, ApimUserType apimUserType)
        {
            return await _apimUserRepository.GetByInternalIdAndType(internalUserId, (int)apimUserType);
        }

        private async Task<string> CreateApimUser(string apimUserId, UserDetails userDetails)
        {
            var apimUserResponse = await _azureApimManagementService.Put<CreateUserResponse>(
                new CreateUserRequest(apimUserId, userDetails));

            if (apimUserResponse.StatusCode != HttpStatusCode.BadRequest)
            {
                return apimUserResponse.Body.Name;
            }

            var getUserResponse = await _azureApimManagementService.Get<ApimUserResponse>(
                new GetApimUserRequest(userDetails.EmailAddress));

            return getUserResponse.Body.Properties.First().Name;

        }
    }
}