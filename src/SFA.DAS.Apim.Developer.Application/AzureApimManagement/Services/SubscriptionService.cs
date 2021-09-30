using System;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        public Task<Guid> CreateUserSubscription(Subscription subscription)
        {
            throw new NotImplementedException();
        }
    }
}