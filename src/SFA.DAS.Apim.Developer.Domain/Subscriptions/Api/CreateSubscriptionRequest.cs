using System.Text.Json.Serialization;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api
{
    public class CreateSubscriptionRequest : IPutRequest
    {
        private readonly string _apimResourceId;
        private readonly string _subscriptionId;

        public CreateSubscriptionRequest(string apimResourceId, string subscriptionId, string subscriberType, string internalUserRef, string apimUserId, string productId)
        {
            _apimResourceId = apimResourceId;
            _subscriptionId = subscriptionId;
            Data = new CreateSubscriptionRequestBody
            {
                Properties = new ApimSubscriptionContract
                {
                    DisplayName = $"{subscriberType}-{internalUserRef}",
                    OwnerId = $"/users/{apimUserId}",
                    Scope = $"/products/{productId}",
                    State = SubscriptionState.Active
                }
            };
        }

        public string PutUrl => $"{_apimResourceId}/subscriptions/{_subscriptionId}?api-version=2021-04-01-preview";
        public object Data { get; set; }
    }

    public class CreateSubscriptionRequestBody
    {
        [JsonPropertyName("properties")]
        public ApimSubscriptionContract Properties { get; set; }
    }

    public class ApimSubscriptionContract
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; }
        [JsonPropertyName("state")]
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