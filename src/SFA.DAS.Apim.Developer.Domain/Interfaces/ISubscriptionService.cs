using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriptionService
    {
        Task<CreateSubscriptionResponse> CreateUserSubscription(string internalUserId, Models.ApimUserType apimUserType, string productName, UserDetails userDetails);
    }
}