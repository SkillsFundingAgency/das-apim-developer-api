using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface ISubscriberTypeRepository
    {
        Task Insert(SubscriberType subscriberType);
        Task<SubscriberType> Get(string name);
        Task<IEnumerable<SubscriberType>> GetAll();
    }
}