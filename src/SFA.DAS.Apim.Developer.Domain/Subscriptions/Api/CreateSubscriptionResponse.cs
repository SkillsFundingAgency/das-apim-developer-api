using System.Text.Json.Serialization;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api
{
    public class CreateSubscriptionResponse
    {
        [JsonPropertyName("properties")]
        public ApimSubscriptionContract Properties { get; set; }

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
    }
}