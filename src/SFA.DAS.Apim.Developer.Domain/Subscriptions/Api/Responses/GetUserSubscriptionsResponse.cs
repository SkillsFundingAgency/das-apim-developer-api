using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses
{
    public class GetUserSubscriptionsResponse
    {
        [JsonProperty("value")]
        public List<UserSubscriptionItem> Value { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("nextLink")]
        public string NextLink { get; set; }
        
    }
    public class UserSubscriptionItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }

    public class Properties
    { 
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}