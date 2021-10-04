using System.Text.Json.Serialization;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api
{
    public class CreateSubscriptionRequest : IPutRequest
    {
        private readonly string _subscriptionId;
        
        public CreateSubscriptionRequest(string subscriptionId, string subscriberType, string internalUserRef, string apimUserId, string productId)
        {
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

        public string PutUrl => $"/subscriptions/{_subscriptionId}?api-version=2021-04-01-preview";
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

        [JsonPropertyName("primaryKey")]
        public string PrimaryKey { get; set; }
        [JsonPropertyName("secondaryKey")]
        public string SecondaryKey { get; set; }
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