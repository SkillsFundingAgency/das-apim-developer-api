using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Extensions;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Responses;
using UserNote = SFA.DAS.Apim.Developer.Domain.Models.UserNote;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        private readonly IAzureUserAuthenticationManagementService _azureUserAuthenticationManagementService;
        private readonly ApimDeveloperApiConfiguration _config;

        public UserService(
            IAzureApimManagementService azureApimManagementService, 
            IAzureUserAuthenticationManagementService azureUserAuthenticationManagementService,
            IOptions<ApimDeveloperApiConfiguration> configOptions)
        {
            _azureApimManagementService = azureApimManagementService;
            _azureUserAuthenticationManagementService = azureUserAuthenticationManagementService;
            _config = configOptions.Value;
        }

        public async Task<UserDetails> UpdateUser(UserDetails userDetails)
        {
            var getUserResponse = await GetUserById(userDetails.Id);

            if (getUserResponse == null)
            {
                return null;
            }
            
            // apim mgmt api required fields: https://docs.microsoft.com/en-us/rest/api/apimanagement/current-ga/user/create-or-update#request-body
            userDetails.Email ??= getUserResponse.Email;
            userDetails.FirstName ??= getUserResponse.FirstName;
            userDetails.LastName ??= getUserResponse.LastName;
            // note cleared if not set
            userDetails.Note ??= getUserResponse.Note;

            var createApimUserTask = await UpsertApimUser(userDetails.Id, userDetails);
            
            return new UserDetails
            {
                Id = createApimUserTask.Name,
                Password = null,
                Email = createApimUserTask.Properties.Email,
                FirstName = createApimUserTask.Properties.FirstName,
                LastName = createApimUserTask.Properties.LastName,
                State = createApimUserTask.Properties.State
            };
        }
        
        public async Task<UserDetails> CreateUser(UserDetails userDetails)
        {
            var getUserResponse = await GetUser(userDetails.Email);

            var userId = userDetails.Id;

            if (getUserResponse != null)
            {
                throw new ValidationException("User already exists");
            }
            
            userDetails.State = "pending";
            var createApimUserTask = await UpsertApimUser(userId, userDetails);
            
            return new UserDetails
            {
                Id = createApimUserTask.Name,
                Password = null,
                Email = createApimUserTask.Properties.Email,
                FirstName = createApimUserTask.Properties.FirstName,
                LastName = createApimUserTask.Properties.LastName,
                State = createApimUserTask.Properties.State
            };
        }

        public async Task<UserDetails> GetUser(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return null;
            }
            
            var result = await _azureApimManagementService.Get<ApimUserResponse>(
                new GetApimUserRequest(emailAddress));

            if (result.Body.Count == 0)
            {
                return null;
            }

            if (!result.Body.Values.First().Properties.Note.TryParseJson(out UserNote userNote))
            {
                userNote =  new UserNote {ConfirmEmailLink = result.Body.Values.First().Properties.Note};
            }
            
            return new UserDetails
            {
                Id = result.Body.Values.First().Name,
                Password = null,
                Email = result.Body.Values.First().Properties.Email,
                FirstName = result.Body.Values.First().Properties.FirstName,
                LastName = result.Body.Values.First().Properties.LastName,
                State = result.Body.Values.First().Properties.State,
                Note = userNote
            };
        }

        public async Task<UserDetails> GetUserById(string id)
        {
            var result = await _azureApimManagementService.Get<ApimUserResponseItem>(
                new GetApimUserByIdRequest(id));

            if (result.StatusCode == HttpStatusCode.NotFound || result.Body == null)
            {
                return null;
            }

            UserNote userNote = null;
            if (result.Body.Properties.Note != null && !result.Body.Properties.Note.TryParseJson(out userNote))
            {
                userNote =  new UserNote {ConfirmEmailLink = result.Body.Properties.Note};
            }

            return new UserDetails
            {
                Id = result.Body.Name,
                Password = null,
                Email = result.Body.Properties.Email,
                FirstName = result.Body.Properties.FirstName,
                LastName = result.Body.Properties.LastName,
                State = result.Body.Properties.State,
                Note = userNote
            };
        }

        private async Task<UserResponse> UpsertApimUser(string apimUserId, UserDetails userDetails)
        {
            var apimUserResponse = await _azureApimManagementService.Put<UserResponse>(
                new CreateUserRequest(apimUserId, userDetails));

            if (!string.IsNullOrEmpty(apimUserResponse.ErrorContent))
            {
                throw new ValidationException(apimUserResponse.ErrorContent);
            }
            
            return apimUserResponse.Body;
        }

        public async Task<UserDetails> CheckUserAuthentication(string email, string password)
        {
            var authenticatedTask =
                _azureUserAuthenticationManagementService.GetAuthentication<GetUserAuthenticatedResponse>(
                    new GetUserAuthenticatedRequest(email, password));
            var userTask = GetUser(email);
            await Task.WhenAll(authenticatedTask, userTask);

            if (userTask.Result == null)
            {
                return null;
            }
            var user = userTask.Result;
            
            if (user.Note.AccountLockedDateTime.HasValue && DateTime.Now.AddMinutes(-_config.AccountLockedDurationMinutes) > user.Note.AccountLockedDateTime)
            {
                user.Note.FailedAuthCount = 0;
                user.Note.AccountLockedDateTime = null;
                user.State = "active";
                await UpsertApimUser(user.Id, user);
                return await CheckUserAuthentication(email, password);
            }

            user.Authenticated = authenticatedTask.Result.StatusCode == HttpStatusCode.OK;
            if (!user.Authenticated && !user.Note.AccountLockedDateTime.HasValue)
            {
                user.Note.FailedAuthCount += 1;
                if (user.Note.FailedAuthCount >= _config.NumberOfAuthFailuresToLockAccount)
                {
                    user.Note.AccountLockedDateTime = DateTime.Now;
                    user.State = "blocked";
                }
                await UpsertApimUser(user.Id, user);
            }

            if (user.Authenticated && user.Note.FailedAuthCount > 0)
            {
                user.Note.FailedAuthCount = 0;
                await UpsertApimUser(user.Id, user);
            }

            return user;
        }
    }
}