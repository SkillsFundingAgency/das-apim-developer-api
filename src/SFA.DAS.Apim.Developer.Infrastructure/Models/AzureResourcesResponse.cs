using System.Collections.Generic;
using Newtonsoft.Json;


namespace SFA.DAS.Apim.Developer.Infrastructure.Models
{
    //https://docs.microsoft.com/en-us/rest/api/resources/resources/list
    public class AzureResourcesResponse
    {
        [JsonProperty("value")]
        public List<AzureResource> AzureResources { get; set; }
    }
    public class AzureResource
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}