using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Products.Api.Responses
{
    public class GetProductsResponse
    {
        [JsonProperty("value")]
        public List<Value> Value { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }
    
    public class Value
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }
    public class Properties
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("groups")]
        public List<Group> Groups { get; set; }
    }
    public class Group
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }
    }
}