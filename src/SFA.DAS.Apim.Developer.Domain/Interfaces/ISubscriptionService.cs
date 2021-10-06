using System;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriptionService
    {
        Task<string> CreateUserSubscription(string internalUserId, string apimUserType, string productName);
    }
}