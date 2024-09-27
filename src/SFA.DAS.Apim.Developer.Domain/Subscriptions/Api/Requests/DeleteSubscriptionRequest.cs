using SFA.DAS.Apim.Developer.Domain.Interfaces;
using System.Web;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
{
    public class DeleteSubscriptionRequest : IDeleteRequest
    {
        private readonly string _subscriptionId;

        public DeleteSubscriptionRequest(string subscriptionId)
        {
            _subscriptionId = subscriptionId;
        }

        public string DeleteUrl => $"subscriptions/{HttpUtility.UrlEncode(_subscriptionId)}?api-version=2023-09-01-preview";
    }
}