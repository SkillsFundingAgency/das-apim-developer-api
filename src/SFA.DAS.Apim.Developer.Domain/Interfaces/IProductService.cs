using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProducts(List<string> allowedGroups);
    }
}