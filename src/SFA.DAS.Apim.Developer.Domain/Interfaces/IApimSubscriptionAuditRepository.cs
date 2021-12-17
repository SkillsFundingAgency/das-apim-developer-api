using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IApimSubscriptionAuditRepository
    {
        Task Insert(ApimSubscriptionAudit apimSubscriptionAudit);

    }
}