using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Data.Repository
{
    public class SubscriberTypeRepository : ISubscriberTypeRepository
    {
        private readonly IApimDeveloperDataContext _apimDeveloperDataContext;

        public SubscriberTypeRepository(IApimDeveloperDataContext apimDeveloperDataContext)
        {
            _apimDeveloperDataContext = apimDeveloperDataContext;
        }

        public async Task<SubscriberType> Get(string name)
        {
            var subscriberType = await _apimDeveloperDataContext
                .SubscriberType
                .SingleOrDefaultAsync(c => c.Name.Equals(name));
            return subscriberType;
        }

        public async Task<IEnumerable<SubscriberType>> GetAll()
        {
            var subscriberTypes = await _apimDeveloperDataContext
                .SubscriberType
                .ToListAsync();

            return subscriberTypes;
        }
    }
}
