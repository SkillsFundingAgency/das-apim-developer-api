using System.Web;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Products.Api.Requests
{
    public class GetProductApiRequest : IGetRequest
    {
        private readonly string _name;

        public GetProductApiRequest (string name)
        {
            _name = HttpUtility.UrlEncode(name);
        }

        public string GetUrl => $"products/{_name}/Apis?api-version=2020-12-01";
    }
}