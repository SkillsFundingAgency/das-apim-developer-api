using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api
{
    public class GetAzureResourcesRequest : IGetRequest
    {
        private readonly string _subscriptionId;
        private readonly string _apimServiceName;

        public GetAzureResourcesRequest(string subscriptionId, string apimServiceName)
        {
            _subscriptionId = subscriptionId;
            _apimServiceName = apimServiceName;
        }

        public string GetUrl =>
            $"subscriptions/{_subscriptionId}/resources?$filter=resourceType eq 'Microsoft.ApiManagement/service' and name eq '{_apimServiceName}'&api-version=2021-04-01";
    }
}