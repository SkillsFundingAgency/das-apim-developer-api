using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
{
    public class GetUserSubscriptionsRequest : IGetRequest
    {
        private readonly string _name;

        public GetUserSubscriptionsRequest (string name)
        {
            _name = name;
        }

        public string GetUrl => $"subscriptions?$filter=name eq '{_name}'&api-version=2021-08-01";
    }
}
