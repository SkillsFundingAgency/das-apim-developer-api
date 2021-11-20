using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses
{
    public class ListAzureApimResourcesRequest : IPostRequest
    {

        public ListAzureApimResourcesRequest(string apimServiceName)
        {
            Data = new ListAzureApimResourcesRequestBody{
                Query = $"where name=~'{apimServiceName}' and type=~'microsoft.apimanagement/service'"
            };
        }

        public string PostUrl => "providers/Microsoft.ResourceGraph/resources?api-version=2021-03-01";

        public object Data { get; set; }

    }

    public class ListAzureApimResourcesRequestBody
    {
        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }
    }
}