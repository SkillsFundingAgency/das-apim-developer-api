using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Products.Api.Responses
{
    public class GetProductApisResponse
    {
        [JsonProperty("value")]
        public List<GetProductApiItem> Value { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public class GetProductApiItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("properties")]
        public ProductApiProperties Properties { get; set; }
    }
    
    public class ProductApiProperties
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}