using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api
{
    public class CreateSubscriptionRequest : IPutRequest
    {
        private readonly string _subscriptionId;

        public CreateSubscriptionRequest(string subscriptionId, string apimUserId, string productId)
        {
            _subscriptionId = subscriptionId;
            Data = new CreateSubscriptionRequestBody
            {
                Properties = new ApimSubscriptionContract
                {
                    DisplayName = _subscriptionId, // TODO: maybe something better?
                    OwnerId = $"/users/{apimUserId}",
                    Scope = $"/products/{productId}",
                    State = SubscriptionState.Active
                }
            };
        }

        public string PutUrl => $"subscriptions/{_subscriptionId}?api-version=2021-04-01-preview";
        public object Data { get; set; }
    }

    public class CreateSubscriptionRequestBody
    {
        [JsonProperty(PropertyName = "properties")]
        public ApimSubscriptionContract Properties { get; set; }
    }

    public class ApimSubscriptionContract
    {
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }
        [JsonProperty(PropertyName = "ownerId")]
        public string OwnerId { get; set; }
        [JsonProperty(PropertyName = "state")]
        public SubscriptionState State { get; set; }
    }

    public enum SubscriptionState
    {
        Active,
        Cancelled,
        Expired,
        Rejected,
        Submitted,
        Suspended
    }
}