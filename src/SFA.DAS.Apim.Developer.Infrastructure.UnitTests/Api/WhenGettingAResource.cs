using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Infrastructure.Api;

namespace SFA.DAS.Apim.Developer.Infrastructure.UnitTests.Api
{
    public class WhenGettingAuthenticationResource
    {
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_Data_Returned(
            int id,
            string responseContent,
            string apimServiceName,
            string resourceId,
            string authToken,
            AzureApimManagementConfiguration azureApimManagementConfiguration)
        {
            //Arrange
            var url = $"https://management.azure.com/{azureApimManagementConfiguration.ApimResourceId}/";
            var tokenProvider = new Mock<IAzureTokenService>();

            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new TestResponse { MyResponse = responseContent })),
                StatusCode = HttpStatusCode.OK
            };
            var getTestRequest = new GetTestRequest(resourceId, id, authToken);
            var expectedUrl = $"{url}{getTestRequest.GetUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(expectedUrl), HttpMethod.Get);
            var client = new HttpClient(httpMessageHandler.Object);
            var azureApimManagementService = new AzureApimManagementService(client, tokenProvider.Object, azureApimManagementConfiguration);
            
            //Act
            var actualResult = await azureApimManagementService.GetAuthentication<TestResponse>(getTestRequest, "application/vnd.oai.openapi+json");

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals(getTestRequest.AuthorizationHeaderScheme)
                        && c.Headers.Authorization.Parameter.Equals(getTestRequest.AuthorizationHeaderValue)
                        && c.Headers.Accept.First().MediaType.Equals("application/vnd.oai.openapi+json")),
                    ItExpr.IsAny<CancellationToken>()
                );

            actualResult.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResult.Body.MyResponse.Should().Be(responseContent);
        }

        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_Error_Content_Returned_If_Error(
            int id,
            string responseContent,
            string resourceId,
            string authToken,
            AzureApimManagementConfiguration azureApimManagementConfiguration)
        {
            //Arrange
            var url = $"https://management.azure.com/{azureApimManagementConfiguration.ApimResourceId}/";
            var tokenProvider = new Mock<IAzureTokenService>();

            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new TestResponse { MyResponse = responseContent })),
                StatusCode = HttpStatusCode.BadRequest
            };
            var getTestRequest = new GetTestRequest(resourceId, id, authToken);
            var expectedUrl = $"{url}{getTestRequest.GetUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(expectedUrl), HttpMethod.Get);
            var client = new HttpClient(httpMessageHandler.Object);
            var azureApimManagementService = new AzureApimManagementService(client, tokenProvider.Object, azureApimManagementConfiguration);
            
            //Act
            var actualResult = await azureApimManagementService.GetAuthentication<TestResponse>(getTestRequest);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals(getTestRequest.AuthorizationHeaderScheme)
                        && c.Headers.Authorization.Parameter.Equals(getTestRequest.AuthorizationHeaderValue)
                        && c.Headers.Accept.First().MediaType.Equals("application/json")),
                    ItExpr.IsAny<CancellationToken>()
                );

            actualResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualResult.ErrorContent.Should().Be(JsonConvert.SerializeObject(new TestResponse { MyResponse = responseContent }));
            actualResult.Body.Should().BeNull();
        }
        
        private class GetTestRequest : IGetUserAuthenticationRequest
        {
            private readonly string _resourceId;
            private readonly int _id;

            public GetTestRequest(string resourceId, int id, string headerValue)
            {
                _resourceId = resourceId;
                _id = id;
                AuthorizationHeaderValue = headerValue;
                AuthorizationHeaderScheme = "Basic";
            }
            public string GetUrl => $"{_resourceId}/test-url/get{_id}";
            public string AuthorizationHeaderValue { get; }
            public string AuthorizationHeaderScheme { get; }
        }
        private class TestResponse
        {
            public string MyResponse { get; set; }
        }
    }
    
    public class WhenGettingAResource
    {
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_Data_Returned(
            int id,
            string responseContent,
            string apimServiceName,
            string resourceId,
            string authToken,
            AzureApimManagementConfiguration azureApimManagementConfiguration)
        {
            //Arrange
            var url = $"https://management.azure.com/{azureApimManagementConfiguration.ApimResourceId}/";
            var tokenProvider = new Mock<IAzureTokenService>();
            tokenProvider.Setup(x => x.GetToken()).ReturnsAsync(authToken);

            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new TestResponse { MyResponse = responseContent })),
                StatusCode = HttpStatusCode.OK
            };
            var getTestRequest = new GetTestRequest(resourceId, id);
            var expectedUrl = $"{url}{getTestRequest.GetUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(expectedUrl), HttpMethod.Get);
            var client = new HttpClient(httpMessageHandler.Object);
            var azureApimManagementService = new AzureApimManagementService(client, tokenProvider.Object, azureApimManagementConfiguration);
            
            //Act
            var actualResult = await azureApimManagementService.Get<TestResponse>(getTestRequest, "application/vnd.oai.openapi+json");

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)
                        && c.Headers.Accept.First().MediaType.Equals("application/vnd.oai.openapi+json")),
                    ItExpr.IsAny<CancellationToken>()
                );

            actualResult.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResult.Body.MyResponse.Should().Be(responseContent);
        }
        
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_Error_Content_Returned_If_Error(
            int id,
            string responseContent,
            string apimServiceName,
            string resourceId,
            string authToken,
            AzureApimManagementConfiguration azureApimManagementConfiguration)
        {
            //Arrange
            var url = $"https://management.azure.com/{azureApimManagementConfiguration.ApimResourceId}/";
            var tokenProvider = new Mock<IAzureTokenService>();
            tokenProvider.Setup(x => x.GetToken()).ReturnsAsync(authToken);

            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new TestResponse { MyResponse = responseContent })),
                StatusCode = HttpStatusCode.InternalServerError
            };
            var getTestRequest = new GetTestRequest(resourceId, id);
            var expectedUrl = $"{url}{getTestRequest.GetUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(expectedUrl), HttpMethod.Get);
            var client = new HttpClient(httpMessageHandler.Object);
            var azureApimManagementService = new AzureApimManagementService(client, tokenProvider.Object, azureApimManagementConfiguration);
            
            //Act
            var actualResult = await azureApimManagementService.Get<TestResponse>(getTestRequest);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );

            actualResult.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            actualResult.ErrorContent.Should().Be(JsonConvert.SerializeObject(new TestResponse { MyResponse = responseContent }));
            actualResult.Body.Should().BeNull();
        }
        
        private class GetTestRequest : IGetRequest
        {
            private readonly string _resourceId;
            private readonly int _id;

            public GetTestRequest(string resourceId, int id)
            {
                _resourceId = resourceId;
                _id = id;
            }
            public string GetUrl => $"{_resourceId}/test-url/get{_id}";
        }
        private class TestResponse
        {
            public string MyResponse { get; set; }
        }
    }
}