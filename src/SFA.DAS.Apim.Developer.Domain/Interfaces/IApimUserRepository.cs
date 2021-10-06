using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IApimUserRepository
    {
        Task<ApimUser> Insert(ApimUser apimUser);
        Task<ApimUser> Get(Guid id);
        Task<ApimUser> GetByInternalIdAndType(string internalUserId, int apimUserTypeId);
        Task<IEnumerable<ApimUser>> GetAll();
    }
}   