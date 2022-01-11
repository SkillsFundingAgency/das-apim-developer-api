using System.Web;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Products.Api.Requests
{
    public class GetProductApiDocumentationRequest : IGetRequest
    {
        private readonly string _apiName;

        public GetProductApiDocumentationRequest(string apiName)
        {
            _apiName = HttpUtility.UrlEncode(apiName);
        }

        public string GetUrl => $"apis/{_apiName}?api-version=2021-04-01-preview";
    }
}