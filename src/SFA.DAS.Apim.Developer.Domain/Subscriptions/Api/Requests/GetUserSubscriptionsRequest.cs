using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
{
    public class GetUserSubscriptionsRequest : IGetRequest
    {
        private readonly string _userId;

        public GetUserSubscriptionsRequest (string userId)
        {
            _userId = userId;
        }

        public string GetUrl => $"subscriptions?$filter=userId eq '{_userId}'&api-version=2021-04-01-preview";
    }
}