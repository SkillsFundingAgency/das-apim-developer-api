using System;
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
                if (value.Properties.Groups.Any(propertiesGroup => allowedGroups.Contains(propertiesGroup.Name, StringComparer.CurrentCultureIgnoreCase)))
                {
                    var apiDetail =
                        await _azureApimManagementService.Get<GetProductApisResponse>(
                            new GetProductApiRequest(value.Name));

                    if (apiDetail.Body.Count == 0)
                    {
                        continue;
                    }

                    var documents = new Dictionary<string, string>();
                    
                    foreach (var getProductApiItem in apiDetail.Body.Value)
                    {
                        var documentation =
                            await _azureApimManagementService.Get<object>(
                                new GetProductApiDocumentationRequest(getProductApiItem.Name), "application/vnd.oai.openapi+json");
                    
                        documents.Add(getProductApiItem.Name.ToLower(),documentation.Body.ToString());
                    }
                    
                    returnList.Add(new Product
                    {
                        Id = value.Name,
                        Name = apiDetail.Body.Value.Last().Name,
                        DisplayName = apiDetail.Body.Value.Last().Properties.DisplayName,
                        Description = apiDetail.Body.Value.Last().Properties.Description,
                        Documentation = documents.Last().Value,
                        Documents = documents
                    });
                    
                }
            }

            return returnList;
        }
    }
}