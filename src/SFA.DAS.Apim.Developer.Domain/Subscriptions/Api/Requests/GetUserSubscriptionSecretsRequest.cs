using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
{
    public class GetUserSubscriptionSecretsRequest : IGetRequest
    {
        private readonly string _subscriptionId;

        public GetUserSubscriptionSecretsRequest (string subscriptionId)
        {
            _subscriptionId = subscriptionId;
        }

        public string GetUrl => $"subscriptions/{_subscriptionId}/listSecrets?api-version=2021-04-01-preview";
    }
}