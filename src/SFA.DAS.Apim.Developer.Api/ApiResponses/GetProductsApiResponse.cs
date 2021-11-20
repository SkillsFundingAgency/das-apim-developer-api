using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetProducts;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Api.ApiResponses
{
    public class GetProductsApiResponse
    {
        public List<GetProductsApiResponseItem> Products { get; set; }

        public static implicit operator GetProductsApiResponse(GetProductsQueryResponse source)
        {
            return new GetProductsApiResponse
            {
                Products = source.Products.Select(c=>(GetProductsApiResponseItem)c).ToList()
            };
        }
    }

    public class GetProductsApiResponseItem
    {
        public string Id { get ; set ; }
        public string Name { get; set; }
        public string DisplayName { get ; set ; }
        public string Description { get ; set ; }


        public static implicit operator GetProductsApiResponseItem(Product source)
        {
            return new GetProductsApiResponseItem
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                DisplayName = source.DisplayName
            };
        }
    }
}