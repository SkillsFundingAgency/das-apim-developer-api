using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Responses;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;

        public ProductService (IAzureApimManagementService azureApimManagementService)
        {
            _azureApimManagementService = azureApimManagementService;
        }
        public async Task<IEnumerable<Product>> GetProducts(List<string> allowedGroups) 
        {
            var products = await _azureApimManagementService.Get<GetProductsResponse>(new GetProductsRequest());

            if (products.Body == null)
            {
                return new List<Product>();
            }
            
            var returnList = new List<Product>();
            foreach (var value in products.Body.Value)
            {
                if (value.Properties.Groups.Any(propertiesGroup => allowedGroups.Contains(propertiesGroup.Name)))
                {
                    returnList.Add(new Product{Name = value.Name});
                }
            }

            return returnList;
        }
    }
}