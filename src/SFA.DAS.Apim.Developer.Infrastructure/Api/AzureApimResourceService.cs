using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Infrastructure.Models;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimResourceService : IAzureApimResourceService
    {
        private readonly HttpClient _client;
        private readonly IAzureTokenService _azureTokenService;
        private readonly IOptions<AzureApimManagementConfiguration> _configuration;

        public AzureApimResourceService(HttpClient client, IAzureTokenService azureTokenService, IOptions<AzureApimManagementConfiguration> configuration)
        {
            _client = client;
            _client.BaseAddress = new Uri($"https://management.azure.com/");
            _azureTokenService = azureTokenService;
            _configuration = configuration;
        }
        
        public async Task<string> GetResourceId()
        {
            var subsRequest = new AzureSubscriptionsRequest
            {
                GetUrl = $"/subscriptions?api-version=2020-01-01"
            };
            var subs = await Get<AzureSubscriptionsResponse>(subsRequest);
            if (subs.value.Count != 1)
            {
                throw new System.Exception("Subscription count unexpected");
            }
            var subscriptionId = subs.value.First().subscriptionId;

            var apimRequest = new AzureResourcesRequest
            {
                GetUrl = $"/subscriptions/{subscriptionId}/resources?$filter=resourceType eq 'Microsoft.ApiManagement/service' and name eq '{_configuration.Value.ApimServiceName}'&api-version=2021-04-01"
            };
            var apimResources = await Get<AzureResourcesReponse>(apimRequest);
            if (apimResources.value.Count != 1)
            {
                throw new System.Exception("Apim Resources count unexpected");
            }

            return apimResources.value.First().id;
        }

        private async Task<T> Get<T>(IGetRequest getRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, getRequest.GetUrl);
            var token = await _azureTokenService.GetToken();
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