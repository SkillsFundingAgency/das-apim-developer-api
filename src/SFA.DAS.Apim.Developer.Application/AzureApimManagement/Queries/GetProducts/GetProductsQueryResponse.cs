using System.Collections.Generic;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetProducts
{
    public class GetProductsQueryResponse
    {
        public IEnumerable<Product> Products { get ; set ; }
    }
}