using System.Collections.Generic;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries
{
    public class GetProductsQueryResponse
    {
        public IEnumerable<Product> Products { get ; set ; }
    }
}