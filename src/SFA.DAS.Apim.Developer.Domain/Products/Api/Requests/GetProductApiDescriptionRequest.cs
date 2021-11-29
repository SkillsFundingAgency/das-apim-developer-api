using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Products.Api.Requests
{
    public class GetProductApiDescriptionRequest : IGetRequest
    {
        private readonly string _apiName;

        public GetProductApiDescriptionRequest(string apiName)
        {
            _apiName = apiName;
        }

        public string GetUrl => $"apis/{_apiName}?api-version=2021-04-01-preview";
    }
}