using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IApimAuditRepository
    {
        Task Insert(ApimAudit apimAudit);
    }
}