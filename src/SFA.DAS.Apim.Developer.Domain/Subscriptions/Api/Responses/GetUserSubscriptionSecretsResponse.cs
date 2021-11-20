using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses
{
    public class GetUserSubscriptionSecretsResponse
    {
        [JsonProperty("primaryKey")]
        public string PrimaryKey { get; set; }

        [JsonProperty("secondaryKey")]
        public string SecondaryKey { get; set; }
    }
}