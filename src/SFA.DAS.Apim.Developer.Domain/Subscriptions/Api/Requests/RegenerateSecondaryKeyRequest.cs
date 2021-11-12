using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
{
    public class RegenerateSecondaryKeyRequest : IPostRequest
    {
        private readonly string _subscriptionId;
        
        public RegenerateSecondaryKeyRequest(string subscriptionId)
        {
            _subscriptionId = subscriptionId;
        }
        
        public string PostUrl => $"subscriptions/{_subscriptionId}/regenerateSecondaryKey?api-version=2021-04-01-preview";
        public object Data { get; set; }
    }
}