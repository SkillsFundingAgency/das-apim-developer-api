using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Data.Repository
{
    public class ApimSubscriptionAuditRepository : IApimSubscriptionAuditRepository
    {
        private readonly IApimDeveloperDataContext _apimDeveloperDataContext;

        public ApimSubscriptionAuditRepository(IApimDeveloperDataContext apimDeveloperDataContext)
        {
            _apimDeveloperDataContext = apimDeveloperDataContext;
        }

        public async Task Insert(ApimSubscriptionAudit apimSubscriptionAudit)
        {
            await _apimDeveloperDataContext.ApimSubscriptionAudit.AddAsync(apimSubscriptionAudit);
            await _apimDeveloperDataContext.SaveChangesAsync();
        }
    }
}
