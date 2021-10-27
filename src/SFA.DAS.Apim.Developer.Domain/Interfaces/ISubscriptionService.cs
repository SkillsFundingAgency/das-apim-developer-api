using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriptionService
    {
        Task<UserSubscription> CreateUserSubscription(string internalUserId,
            ApimUserType apimUserType, string productName, UserDetails userDetails);
    }
}