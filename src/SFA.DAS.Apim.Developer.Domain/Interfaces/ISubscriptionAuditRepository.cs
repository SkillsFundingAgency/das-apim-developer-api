using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriptionAuditRepository
    {
        Task Insert(SubscriptionAudit subscriptionAudit);
    }
}