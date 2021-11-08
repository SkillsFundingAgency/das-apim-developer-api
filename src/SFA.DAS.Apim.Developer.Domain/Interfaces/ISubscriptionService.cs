using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriptionService
    {
        Task<UserSubscription> CreateUserSubscription(string internalUserId,
            ApimUserType apimUserType, string productName, UserDetails userDetails);

        Task<IEnumerable<UserSubscription>> GetUserSubscriptions(string internalUserId, ApimUserType apimUserType);
    }
}