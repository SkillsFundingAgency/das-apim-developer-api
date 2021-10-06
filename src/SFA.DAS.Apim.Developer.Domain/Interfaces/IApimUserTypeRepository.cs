using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IApimUserTypeRepository
    {
        Task<ApimUserType> Get(string name);
        Task<IEnumerable<ApimUserType>> GetAll();
    }
}