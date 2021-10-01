using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Infrastructure.Models;
using System.Linq;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimManagementService : IAzureApimManagementService
    {

        private const string AzureResource = "https://management.azure.com/";
        private string AzureApimResourceId;
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;
        private readonly HttpClient _client;
        private readonly IOptions<AzureApimManagementConfiguration> _configuration;

        public AzureApimManagementService(HttpClient client, IOptions<AzureApimManagementConfiguration> configuration, AzureServiceTokenProvider azureServiceTokenProvider)
        {
            _azureServiceTokenProvider = azureServiceTokenProvider;
            _client = client;
            _configuration = configuration;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken().Result); // TODO: do this better
            SetupAzureParameters(); // TODO: do this better
        }

        public async Task CreateSubscription(string subscriptionId, string subscriberType, string internalUserRef, string apimUserId, string productId)
        {
            var uri = $"{AzureResource}{AzureApimResourceId}/subscriptions/{subscriptionId}?api-version=2021-04-01-preview";
            var body = new PutSubscriptionRequest
            {
                properties = new PutSubscriptionRequest.PutSubscriptionRequestProperties
                {
                    displayName = $"{subscriberType}-{internalUserRef}",  // TODO: think about this more
                    ownerId = $"/users/{apimUserId}",
                    scope = $"/products/{productId}",
                    state = SubscriptionState.active.ToString()
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            using (var response = await _client.PutAsync(uri, content))
            {
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
            }
        }


        private async Task<string> GetToken()
        {
            var token = await _azureServiceTokenProvider.GetAccessTokenAsync(AzureResource);
            return token;
        }

        // TODO: do all of this much better
        private async Task SetupAzureParameters()
        {
            var subscriptionsUri = $"{AzureResource}/subscriptions?api-version=2020-01-01";
            var subs = new AzureSubscriptionsResponse();
            using (var response = await _client.GetAsync(subscriptionsUri))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                subs = JsonConvert.DeserializeObject<AzureSubscriptionsResponse>(responseString);

            }
            if (subs.value.Count != 1)
            {
                throw new System.Exception("Subscription count unexpected");
            }

            var subscriptionId = subs.value.First().subscriptionId;

            var apimResourceUri = $"{AzureResource}/subscriptions/{subscriptionId}/resources?$filter=resourceType eq 'Microsoft.ApiManagement/service' and name eq '{_configuration.Value.ApimServiceName}'&api-version=2021-04-01";
            var apimResources = new AzureResourcesReponse();
            using (var response = await _client.GetAsync(apimResourceUri))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                apimResources = JsonConvert.DeserializeObject<AzureResourcesReponse>(responseString);

            }
            if (apimResources.value.Count != 1)
            {
                throw new System.Exception("Apim Resources count unexpected");
            }

            AzureApimResourceId = apimResources.value.First().id;
        }
    }
}