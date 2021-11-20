using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses
{
    public class CreateSubscriptionResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

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

            [JsonProperty(PropertyName = "primaryKey")]
            public string PrimaryKey { get; set; }

            [JsonProperty(PropertyName = "secondaryKey")]
            public string SecondaryKey { get; set; }
        }
    }
}