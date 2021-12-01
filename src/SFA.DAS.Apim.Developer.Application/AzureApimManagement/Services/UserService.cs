using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Responses;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        

        public UserService(IAzureApimManagementService azureApimManagementService)
        {
            _azureApimManagementService = azureApimManagementService;
        }
        
        public async Task<UserDetails> CreateUser(UserDetails userDetails)
        {
            var getUserResponse = await GetUser(userDetails.Email);

            if (getUserResponse != null)
            {
                return getUserResponse;
            }
            
            var createApimUserTask = await CreateApimUser(Guid.NewGuid().ToString(), userDetails);

                
            return new UserDetails
            {
                Id = createApimUserTask.Name,
                Password = null,
                Email = createApimUserTask.Properties.Email,
                FirstName = createApimUserTask.Properties.FirstName,
                LastName = createApimUserTask.Properties.LastName,
            };;

        }

        public async Task<UserDetails> GetUser(string emailAddress)
        {
            var result = await _azureApimManagementService.Get<ApimUserResponse>(
                new GetApimUserRequest(emailAddress));

            if (result.Body.Count == 0)
            {
                return null;
            }
            
            return new UserDetails
            {
                Id = result.Body.Properties.First().Name,
                Password = null,
                Email = result.Body.Properties.First().Email,
                FirstName = result.Body.Properties.First().FirstName,
                LastName = result.Body.Properties.First().LastName,
            };
        }

        private async Task<CreateUserResponse> CreateApimUser(string apimUserId, UserDetails userDetails)
        {
            var apimUserResponse = await _azureApimManagementService.Put<CreateUserResponse>(
                new CreateUserRequest(apimUserId, userDetails));

            if (!string.IsNullOrEmpty(apimUserResponse.ErrorContent))
            {
                throw new Exception(apimUserResponse.ErrorContent);
            }
            
            return apimUserResponse.Body;
           
        }
    }
}