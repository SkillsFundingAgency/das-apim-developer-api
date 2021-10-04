using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api
{
    public class CreateSubscriptionResponse
    {
        [JsonProperty(PropertyName = "properties")]
        public ApimSubscriptionContract Properties { get; set; }

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
    }
}