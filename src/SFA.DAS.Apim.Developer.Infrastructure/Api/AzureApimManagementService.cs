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
            SetupAzureParameters(); // TODO: do this better
        }

        public async Task CreateSubscription(string subscriptionId, string subscriberType, string internalUserRef, string apimUserId, string productId)
        {
            var request = new PutRequest
            {
                Url = $"{AzureResource}{AzureApimResourceId}/subscriptions/{subscriptionId}?api-version=2021-04-01-preview",
                Body = new PutSubscriptionRequest
                {
                    properties = new ApimSubscriptionContract
                    {
                        displayName = $"{subscriberType}-{internalUserRef}",  // TODO: think about this more
                        ownerId = $"/users/{apimUserId}",
                        scope = $"/products/{productId}",
                        state = SubscriptionState.active
                    }
                }
            };
            var response = await Put<PutSubscriptionResponse>(request);
        }


        private async Task<string> GetToken()
        {
            var token = await _azureServiceTokenProvider.GetAccessTokenAsync(AzureResource);
            return token;
        }

        // TODO: do all of this much better
        private async Task SetupAzureParameters()
        {
            var subsRequest = new GetRequest
            {
                Url = $"{AzureResource}/subscriptions?api-version=2020-01-01"
            };
            var subs = await Get<AzureSubscriptionsResponse>(subsRequest);
            if (subs.value.Count != 1)
            {
                throw new System.Exception("Subscription count unexpected");
            }
            var subscriptionId = subs.value.First().subscriptionId;

            var apimRequest = new GetRequest
            {
                Url = $"{AzureResource}/subscriptions/{subscriptionId}/resources?$filter=resourceType eq 'Microsoft.ApiManagement/service' and name eq '{_configuration.Value.ApimServiceName}'&api-version=2021-04-01"
            };
            var apimResources = await Get<AzureResourcesReponse>(apimRequest);
            if (apimResources.value.Count != 1)
            {
                throw new System.Exception("Apim Resources count unexpected");
            }

            AzureApimResourceId = apimResources.value.First().id;
        }

        private async Task<T> Get<T>(GetRequest getRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, getRequest.Url);
            var token = await GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseString);
            }
        }

        private async Task<T> Put<T>(PutRequest putRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, putRequest.Url);
            request.Content = new StringContent(JsonConvert.SerializeObject(putRequest.Body), Encoding.UTF8, "application/json");

            var token = await GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseString);
            }
        }
    }
}