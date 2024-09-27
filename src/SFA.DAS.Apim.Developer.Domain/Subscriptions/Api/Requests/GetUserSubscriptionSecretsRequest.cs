using System.Web;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
{
    public class GetUserSubscriptionSecretsRequest : IPostRequest
    {
        private readonly string _subscriptionId;

        public GetUserSubscriptionSecretsRequest (string subscriptionId)
        {
            _subscriptionId = HttpUtility.UrlEncode(subscriptionId);
            Data = new object();
        }

        public string PostUrl => $"subscriptions/{_subscriptionId}/listSecrets?api-version=2023-09-01-preview";
        public object Data { get; set; }
    }
}