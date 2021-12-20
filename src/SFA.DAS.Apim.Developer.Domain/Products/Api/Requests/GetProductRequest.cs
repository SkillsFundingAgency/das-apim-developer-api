using System.Web;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Products.Api.Requests
{
    public class GetProductRequest : IGetRequest
    {
        private readonly string _id;

        public GetProductRequest(string id)
        {
            _id = HttpUtility.UrlEncode(id);
        }

        public string GetUrl => $"products/{_id}?api-version=2021-08-01";
    }
}