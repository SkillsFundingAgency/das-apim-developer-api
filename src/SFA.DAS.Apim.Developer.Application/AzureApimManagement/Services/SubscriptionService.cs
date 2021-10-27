using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using ApimUserType = SFA.DAS.Apim.Developer.Domain.Models.ApimUserType;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        private readonly IUserService _userService;

        public SubscriptionService(IAzureApimManagementService azureApimManagementService, IUserService userService)
        {
            _azureApimManagementService = azureApimManagementService;
            _userService = userService;
        }
        
        public async Task<UserSubscription> CreateUserSubscription(string internalUserId,
            ApimUserType apimUserType, string productName, UserDetails userDetails)
        {
            var apimUserId = await _userService.CreateUser(internalUserId, userDetails, apimUserType);
            
            var createSubscriptionRequest = new CreateSubscriptionRequest($"{apimUserType}-{internalUserId}", apimUserId, productName);
            var apiResponse = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);
            
            var createSandboxSubscriptionRequest = new CreateSubscriptionRequest($"{apimUserType}-{internalUserId}-sandbox", apimUserId, productName);
            var sandboxApiResponse = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSandboxSubscriptionRequest);

            var subscription = new UserSubscription
            {
                Id = apiResponse.Body.Id,
                Name = apiResponse.Body.Name,
                PrimaryKey = apiResponse.Body.Properties.PrimaryKey,
                SandboxPrimaryKey = sandboxApiResponse.Body.Properties.PrimaryKey
            };
            
            return subscription;
        }
    }
}