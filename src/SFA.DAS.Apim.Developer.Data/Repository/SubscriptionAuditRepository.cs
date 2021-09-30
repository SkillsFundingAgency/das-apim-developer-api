using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Data.Repository
{
    public class SubscriptionAuditRepository : ISubscriptionAuditRepository
    {
        private readonly IApimDeveloperDataContext _apimDeveloperDataContext;

        public SubscriptionAuditRepository(IApimDeveloperDataContext apimDeveloperDataContext)
        {
            _apimDeveloperDataContext = apimDeveloperDataContext;
        }

        public async Task Insert(SubscriptionAudit subscriptionAudit)
        {
            await _apimDeveloperDataContext.SubscriptionAudit.AddAsync(subscriptionAudit);
            _apimDeveloperDataContext.SaveChanges();
        }
    }
}
