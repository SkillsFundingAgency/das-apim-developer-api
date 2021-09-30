using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Guid> Insert(Subscription subscription);
        Task<Subscription> Get(Guid id);
        Task<IEnumerable<Subscription>> GetAll();
    }
}   