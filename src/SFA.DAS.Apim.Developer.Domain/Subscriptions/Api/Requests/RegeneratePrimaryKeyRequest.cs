using System.Web;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
{
    public class RegeneratePrimaryKeyRequest : IPostRequest
    {
        private readonly string _subscriptionId;
        
        public RegeneratePrimaryKeyRequest(string subscriptionId)
        {
            _subscriptionId = HttpUtility.UrlEncode(subscriptionId);
            Data = new { };
        }
        
        public string PostUrl => $"subscriptions/{_subscriptionId}/regeneratePrimaryKey?api-version=2023-09-01-preview";
        public object Data { get; set; }
    }
}