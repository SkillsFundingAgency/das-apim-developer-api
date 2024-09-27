using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Products.Api.Requests
{
    public class GetProductsRequest : IGetRequest
    {
        public string GetUrl => "products?expandGroups=true&api-version=2023-09-01-preview";
    }
}