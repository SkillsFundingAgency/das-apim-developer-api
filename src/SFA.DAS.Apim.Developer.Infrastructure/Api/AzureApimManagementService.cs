using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using System.Net;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimManagementService : IAzureApimManagementService
    {

        private readonly IAzureTokenService _azureTokenService;
        private readonly HttpClient _client;

        public AzureApimManagementService(HttpClient client, IAzureTokenService azureTokenService, ApimResourceConfiguration resourceConfiguration)
        {
            _azureTokenService = azureTokenService;
            _client = client;
            _client.BaseAddress = new Uri($"https://management.azure.com/{resourceConfiguration.ApimResourceId}/");
        }

        public async Task<T> Get<T>(IGetRequest getRequest)
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

            if (IsNot200RangeResponseCode(response.StatusCode))
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