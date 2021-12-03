using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Responses;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        private readonly IAzureUserAuthenticationManagementService _azureUserAuthenticationManagementService;


        public UserService(IAzureApimManagementService azureApimManagementService, IAzureUserAuthenticationManagementService azureUserAuthenticationManagementService)
        {
            _azureApimManagementService = azureApimManagementService;
            _azureUserAuthenticationManagementService = azureUserAuthenticationManagementService;
        }
        
        public async Task<UserDetails> UpsertUser(UserDetails userDetails)
        {
            var getUserResponse = await GetUser(userDetails.Email);

            var userId = Guid.NewGuid().ToString();
            
            if (getUserResponse != null)
            {
                userId = getUserResponse.Id;
            }
            else
            {
                userDetails.State = "pending";
            }
            
            var createApimUserTask = await CreateApimUser(userId, userDetails);

                
            return new UserDetails
            {
                Id = createApimUserTask.Name,
                Password = null,
                Email = createApimUserTask.Properties.Email,
                FirstName = createApimUserTask.Properties.FirstName,
                LastName = createApimUserTask.Properties.LastName,
                State = createApimUserTask.Properties.State
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
                Id = result.Body.Values.First().Name,
                Password = null,
                Email = result.Body.Values.First().Properties.Email,
                FirstName = result.Body.Values.First().Properties.FirstName,
                LastName = result.Body.Values.First().Properties.LastName,
                State = result.Body.Values.First().Properties.State
            };
        }

        private async Task<UserResponse> CreateApimUser(string apimUserId, UserDetails userDetails)
        {
            var apimUserResponse = await _azureApimManagementService.Put<UserResponse>(
                new CreateUserRequest(apimUserId, userDetails));

            if (!string.IsNullOrEmpty(apimUserResponse.ErrorContent))
            {
                throw new Exception(apimUserResponse.ErrorContent);
            }
            
            return apimUserResponse.Body;
           
        }

        public async Task UpdateUserState(string email)
        {
            var user = await GetUser(email);

            if (user == null)
            {
                throw new ValidationException("User not found");
            }
            
            await _azureApimManagementService.Put<UserResponse>(new UpdateUserStateRequest(user.Id, user));
        }

        public async Task<UserDetails> CheckUserAuthentication(string email, string password)
        {
            var authenticated =
                await _azureUserAuthenticationManagementService.GetAuthentication<GetUserAuthenticatedResponse>(
                    new GetUserAuthenticatedRequest(email, password));

            if (authenticated.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            return await GetUser(email);
        }
    }
}