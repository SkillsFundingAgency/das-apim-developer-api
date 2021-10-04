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
        private readonly IAzureApimResourceService _azureApimResourceService;
        private readonly HttpClient _client;
        private readonly IOptions<AzureApimManagementConfiguration> _configuration;

        public AzureApimManagementService(HttpClient client, IOptions<AzureApimManagementConfiguration> configuration, IAzureTokenService azureTokenService, IAzureApimResourceService azureApimResourceService)
        {
            _azureTokenService = azureTokenService;
            _azureApimResourceService = azureApimResourceService;
            _client = client;
            _client.BaseAddress = new Uri("https://management.azure.com/");
            _configuration = configuration;
        }
        
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
            var resourceId = await _azureApimResourceService.GetResourceId();
            
            var request = new HttpRequestMessage(HttpMethod.Put, $"{resourceId}/{putRequest.PutUrl}")
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

    }
}