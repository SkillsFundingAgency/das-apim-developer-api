using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Data.Repository
{
    public class ApimAuditRepository : IApimAuditRepository
    {
        private readonly IApimDeveloperDataContext _apimDeveloperDataContext;

        public ApimAuditRepository(IApimDeveloperDataContext apimDeveloperDataContext)
        {
            _apimDeveloperDataContext = apimDeveloperDataContext;
        }

        public async Task Insert(ApimAudit apimAudit)
        {
            await _apimDeveloperDataContext.ApimAudit.AddAsync(apimAudit);
            await _apimDeveloperDataContext.SaveChangesAsync();
        }
    }
}
