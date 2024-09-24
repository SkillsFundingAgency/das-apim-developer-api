using System.Web;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
{
    public class GetUserSubscriptionsRequest : IGetRequest
    {
        private readonly string _name;

        public GetUserSubscriptionsRequest (string name)
        {
            _name = HttpUtility.UrlEncode(name);
        }

        public string GetUrl => $"subscriptions?$filter=startswith(name,'{_name}')&api-version=2023-09-01-preview";
    }
}
