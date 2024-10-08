using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
{
    public class CreateSubscriptionRequest : IPutRequest
    {
        private readonly string _subscriptionId;

        public CreateSubscriptionRequest(string subscriptionId, string productId)
        {
            _subscriptionId = subscriptionId;
            Data = new CreateSubscriptionRequestBody
            {
                Properties = new ApimSubscriptionContract
                {
                    DisplayName = _subscriptionId,
                    Scope = $"/products/{productId}",
                    State = SubscriptionState.Active
                }
            };
        }

        public string PutUrl => $"subscriptions/{HttpUtility.UrlEncode(_subscriptionId)}?api-version=2023-09-01-preview";
        public object Data { get; set; }
    }

    public class CreateSubscriptionRequestBody
    {
        [JsonProperty(PropertyName = "properties")]
        public ApimSubscriptionContract Properties { get; set; }
    }

    public class ApimSubscriptionContract
    {
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }
        [JsonProperty(PropertyName = "state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SubscriptionState State { get; set; }
    }

    public enum SubscriptionState
    {
        Active,
        Cancelled,
        Expired,
        Rejected,
        Submitted,
        Suspended
    }
}