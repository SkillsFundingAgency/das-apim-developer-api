using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Subscription> CreateSubscription(string internalUserId,
            ApimUserType apimUserType, string productName);
        Task RegenerateSubscriptionKeys( string internalUserId, ApimUserType apimUserType, string productName);
        Task<IEnumerable<Subscription>> GetUserSubscriptions(string internalUserId, ApimUserType apimUserType);
        Task DeleteSubscription(string internalUserId, ApimUserType apimUserType, string productName);
    }
}