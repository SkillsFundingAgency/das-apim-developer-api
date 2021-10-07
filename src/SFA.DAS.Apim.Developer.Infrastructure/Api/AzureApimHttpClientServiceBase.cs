using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public abstract class AzureApimHttpClientServiceBase
    {
        private readonly IAzureTokenService _azureTokenService;
        private readonly HttpClient _httpClient;

        protected AzureApimHttpClientServiceBase(IAzureTokenService azureTokenService, HttpClient httpClient, string baseAddress)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseAddress);
            _azureTokenService = azureTokenService;
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

            var response = await _httpClient.SendAsync(request);

            return await ResponseHandler<T>(response);
        }

        public async Task<ApiResponse<T>> Get<T>(IGetRequest getRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, getRequest.GetUrl);

            var token = await _azureTokenService.GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            return await ResponseHandler<T>(response);
        }

        private static bool IsNot200RangeResponseCode(HttpStatusCode statusCode)
        {
            return !((int)statusCode >= 200 && (int)statusCode <= 299);
        }

        private async Task<ApiResponse<T>> ResponseHandler<T>(HttpResponseMessage response)
        {
            var responseBody = (T)default;
            var errorContent = "";
            var responseString = await response.Content.ReadAsStringAsync();
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
    }
}