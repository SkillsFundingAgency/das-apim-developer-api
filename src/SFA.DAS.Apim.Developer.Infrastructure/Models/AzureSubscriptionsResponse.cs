using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Infrastructure.Models
{
    //https://docs.microsoft.com/en-us/rest/api/resources/subscriptions/list
    public class AzureSubscriptionsResponse
    {
        public class AzureSubscription
        {
            [JsonProperty("displayName")]
            public string DisplayName { get; set; }
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("subscriptionId")]
            public string SubscriptionId { get; set; }
        }

        [JsonProperty("value")]
        public List<AzureSubscription> AzureSubscriptions { get; set; }
    }
}