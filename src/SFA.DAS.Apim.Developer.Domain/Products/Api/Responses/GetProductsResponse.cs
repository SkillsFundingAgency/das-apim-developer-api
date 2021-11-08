using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Products.Api.Responses
{
    public class GetProductsResponse
    {
        [JsonProperty("value")]
        public List<GetProductItem> Value { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }

}