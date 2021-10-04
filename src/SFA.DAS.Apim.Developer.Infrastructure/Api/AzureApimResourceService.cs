using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using SFA.DAS.Apim.Developer.Infrastructure.Models;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimResourceService : IAzureApimResourceService
    {
        private readonly HttpClient _client;
        private readonly IAzureTokenService _azureTokenService;
        private readonly AzureApimManagementConfiguration _configuration;

        public AzureApimResourceService(HttpClient client, IAzureTokenService azureTokenService, IOptions<AzureApimManagementConfiguration> configuration)
        {
            _client = client;
            _client.BaseAddress = new Uri($"https://management.azure.com/");
            _azureTokenService = azureTokenService;
            _configuration = configuration.Value;
        }

        public async Task<string> GetResourceId()
        {
            var subs = await Get<AzureSubscriptionsResponse>(new GetAzureSubscriptionsRequest());
            if (subs.value.Count != 1)
            {
                throw new Exception("Subscription count unexpected");
            }

            var apimResources = await Get<AzureResourcesResponse>(new GetAzureResourcesRequest(subs.value.First().subscriptionId, _configuration.ApimServiceName));
            
            if (apimResources.AzureResources.Count != 1)
            {
                throw new Exception("Apim Resources count unexpected");
            }

            return apimResources.AzureResources.First().Id;
        }

        private async Task<T> Get<T>(IGetRequest getRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, getRequest.GetUrl);
            
            var token = await _azureTokenService.GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseString);
        }
    }
}