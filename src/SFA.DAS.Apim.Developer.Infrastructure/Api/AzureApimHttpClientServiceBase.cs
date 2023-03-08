using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Extensions;
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

            await AddAuthorization(request);

            var response = await _httpClient.SendAsync(request);

            return await ResponseHandler<T>(response);
        }
        
        public async Task<ApiResponse<T>> Post<T>(IPostRequest postRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, postRequest.PostUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(postRequest.Data), Encoding.UTF8,
                    "application/json")
            };

            await AddAuthorization(request);

            var response = await _httpClient.SendAsync(request);

            return await ResponseHandler<T>(response);
        }

        public async Task<ApiResponse<T>> Get<T>(IGetRequest getRequest, string requestEncoding = "application/json")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, getRequest.GetUrl)
            {
                Headers = { Accept = { new MediaTypeWithQualityHeaderValue(requestEncoding) }}
            };
            
            await AddAuthorization(request);

            var response = await _httpClient.SendAsync(request);

            return await ResponseHandler<T>(response);
        }

        public async Task<ApiResponse<T>> Delete<T>(IDeleteRequest deleteRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, deleteRequest.DeleteUrl)
            {
                Headers = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };

            await AddAuthorization(request);

            var response = await _httpClient.SendAsync(request);

            return await ResponseHandler<T>(response);
        }

        public async Task<ApiResponse<T>> GetAuthentication<T>(IGetUserAuthenticationRequest getUserAuthenticationRequest, string requestEncoding = "application/json")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, getUserAuthenticationRequest.GetUrl)
            {
                Headers =
                {
                    Accept = { new MediaTypeWithQualityHeaderValue(requestEncoding) },
                    Authorization = new AuthenticationHeaderValue(getUserAuthenticationRequest.AuthorizationHeaderScheme, getUserAuthenticationRequest.AuthorizationHeaderValue)
                }
            };
            
            var response = await _httpClient.SendAsync(request);

            return await ResponseHandler<T>(response);
        }

        private async Task<ApiResponse<T>> ResponseHandler<T>(HttpResponseMessage response)
        {
            var responseBody = (T)default;
            var errorContent = "";
            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.StatusCode.IsSuccessStatusCode())
            {
                errorContent = responseString;
            }
            else
            {
                responseBody = JsonConvert.DeserializeObject<T>(responseString);
            }
            return new ApiResponse<T>(responseBody, response.StatusCode, errorContent);
        }

        private async Task AddAuthorization(HttpRequestMessage httpRequestMessage)
        {
            var token = await _azureTokenService.GetToken();
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}