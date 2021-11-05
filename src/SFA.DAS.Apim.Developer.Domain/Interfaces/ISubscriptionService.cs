using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Subscription> CreateSubscription(string internalUserId,
            ApimUserType apimUserType, string productName);
    }
}