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
        public async Task<CreateSubscriptionResponse> CreateUserSubscription(string internalUserId, ApimUserType apimUserType, string productName, UserDetails userDetails)
        {
            var apimUserId = await _userService.CreateUser(internalUserId, userDetails, apimUserType);
            
            var createSubscriptionRequest = new CreateSubscriptionRequest($"{apimUserType}-{internalUserId}", apimUserId, productName);
            
            var response = await _azureApimManagementService.Put<CreateSubscriptionResponse>(createSubscriptionRequest);
            
            return response.Body;
        }
    }
}