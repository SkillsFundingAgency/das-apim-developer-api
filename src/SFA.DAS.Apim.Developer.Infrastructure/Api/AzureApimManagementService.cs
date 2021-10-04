using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Infrastructure.Models;
using System.Linq;
using System.Net;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimManagementService : IAzureApimManagementService
    {

        private const string AzureResource = "https://management.azure.com/";
        private string AzureApimResourceId;
        private readonly IAzureTokenService _azureTokenService;
        private readonly HttpClient _client;
        private readonly IOptions<AzureApimManagementConfiguration> _configuration;

        public AzureApimManagementService(HttpClient client, IOptions<AzureApimManagementConfiguration> configuration, IAzureTokenService azureTokenService)
        {
            _azureTokenService = azureTokenService;
            _client = client;
            _client.BaseAddress = new Uri("https://management.azure.com/");
            _configuration = configuration;
            //SetupAzureParameters(); // TODO: do this better
        }

        // public async Task CreateSubscription(string subscriptionId, string subscriberType, string internalUserRef, string apimUserId, string productId)
        // {
        //     var request = new PutRequest
        //     {
        //         Url = $"{AzureResource}{AzureApimResourceId}/subscriptions/{subscriptionId}?api-version=2021-04-01-preview",
        //         Body = new PutSubscriptionRequest
        //         {
        //             properties = new ApimSubscriptionContract
        //             {
        //                 displayName = $"{subscriberType}-{internalUserRef}",  // TODO: think about this more
        //                 ownerId = $"/users/{apimUserId}",
        //                 scope = $"/products/{productId}",
        //                 state = SubscriptionState.active
        //             }
        //         }
        //     };
        //     var response = await Put<PutSubscriptionResponse>(request);
        // }


        public async Task<T> Get<T>(GetRequest getRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, getRequest.Url);
            var token = await _azureTokenService.GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using (var response = await _client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseString);
            }
        }

        public async Task<ApiResponse<T>> Put<T>(IPutRequest putRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, putRequest.PutUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(putRequest.Data), Encoding.UTF8,
                    "application/json")
            };

            var token = await _azureTokenService.GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var errorContent = "";
            var responseBody = (T)default;
            
            if(IsNot200RangeResponseCode(response.StatusCode))
            {
                errorContent = responseString;
            }
            else
            {
                responseBody = JsonConvert.DeserializeObject<T>(responseString);
            }
            
            return new ApiResponse<T>(responseBody, response.StatusCode, errorContent);
        }

        private static bool IsNot200RangeResponseCode(HttpStatusCode statusCode)
        {
            return !((int)statusCode >= 200 && (int)statusCode <= 299);
        }

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
    }
}