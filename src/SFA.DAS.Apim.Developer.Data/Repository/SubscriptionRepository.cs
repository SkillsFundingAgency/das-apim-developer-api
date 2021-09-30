using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Data.Repository
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IApimDeveloperDataContext _apimDeveloperDataContext;

        public SubscriptionRepository(IApimDeveloperDataContext apimDeveloperDataContext)
        {
            _apimDeveloperDataContext = apimDeveloperDataContext;
        }

        public async Task<Guid> Insert(Subscription subscription)
        {
            await _apimDeveloperDataContext.Subscription.AddAsync(subscription);
            _apimDeveloperDataContext.SaveChanges();
            return subscription.Id;
        }
        public async Task<Subscription> Get(Guid id)
        {
            var subscription = await _apimDeveloperDataContext
                .Subscription
                .FindAsync(id);
            return subscription;
        }

        public async Task<IEnumerable<Subscription>> GetAll()
        {
            var subscriptions = await _apimDeveloperDataContext
                .Subscription
                .ToListAsync();

            return subscriptions;
        }
    }
}
