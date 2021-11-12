using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Subscription> CreateSubscription(string internalUserId,
            ApimUserType apimUserType, string productName);
        Task RegenerateSubscription(
            string internalUserId, ApimUserType apimUserType);
        Task<IEnumerable<Subscription>> GetUserSubscriptions(string internalUserId, ApimUserType apimUserType);
    }
}