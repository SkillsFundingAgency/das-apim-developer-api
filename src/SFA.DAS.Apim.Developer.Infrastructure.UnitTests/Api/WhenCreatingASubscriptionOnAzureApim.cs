using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Infrastructure.Api;

namespace SFA.DAS.Apim.Developer.Infrastructure.UnitTests.Api
{
    public class WhenCreatingASubscriptionOnAzureApim
    {
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_Subscription_Created(
            int id,
            string putContent,
            string responseContent,
            string apimServiceName,
            string resourceId,
            string authToken)
        {
         
            //Arrange
            var url = "https://management.azure.com/";
            var tokenProvider = new Mock<IAzureTokenService>();
            tokenProvider.Setup(x => x.GetToken()).ReturnsAsync(authToken);
            var azureApimResourceService = new Mock<IAzureApimResourceService>();
            azureApimResourceService.Setup(x => x.GetResourceId()).ReturnsAsync(resourceId);
            
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new TestResponse{MyResponse = responseContent})),
                StatusCode = HttpStatusCode.Created
            };
            var putTestRequest = new PutTestRequest(id)
            {
                Data = putContent
            };
            var expectedUrl = $"{url}{resourceId}/{putTestRequest.PutUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(expectedUrl), HttpMethod.Put);
            var client = new HttpClient(httpMessageHandler.Object);
            var azureApimManagementService = new AzureApimManagementService(client, tokenProvider.Object, azureApimResourceService.Object);

            //Act
            var actualResult = await azureApimManagementService.Put<TestResponse>(putTestRequest);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Put)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );
            
            actualResult.StatusCode.Should().Be(HttpStatusCode.Created);
            actualResult.Body.MyResponse.Should().Be(responseContent);

        }
        
        
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_ErrorDescription_Returned_With_Response_Code_If_Not_Created(
            int id,
            string putContent,
            string responseContent,
            string apimServiceName,
            string resourceId,
            string authToken)
        {
            //Arrange
            var url = "https://management.azure.com/";
            var tokenProvider = new Mock<IAzureTokenService>();
            tokenProvider.Setup(x => x.GetToken()).ReturnsAsync(authToken);
            var azureApimResourceService = new Mock<IAzureApimResourceService>();
            azureApimResourceService.Setup(x => x.GetResourceId()).ReturnsAsync(resourceId);
            
            var responseObject = JsonConvert.SerializeObject(new TestResponse{MyResponse = responseContent});
            var response = new HttpResponseMessage
            {
                Content = new StringContent(responseObject),
                StatusCode = HttpStatusCode.BadRequest
            };
            var putTestRequest = new PutTestRequest(id)
            {
                Data = putContent
            };
            var expectedUrl = $"{url}{resourceId}/{putTestRequest.PutUrl}";
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(expectedUrl), HttpMethod.Put);
            var client = new HttpClient(httpMessageHandler.Object);
            var azureApimManagementService = new AzureApimManagementService(client, tokenProvider.Object, azureApimResourceService.Object);

            //Act
            var actualResult = await azureApimManagementService.Put<TestResponse>(putTestRequest);

            //Assert
            httpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Put)
                        && c.RequestUri.AbsoluteUri.Equals(expectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );
            
            actualResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualResult.Body.Should().BeNull();
            actualResult.ErrorContent.Should().Be(responseObject);
        }
        private class PutTestRequest : IPutRequest
        {
            private readonly int _id;

            public PutTestRequest (int id)
            {
                _id = id;
            }
            public object Data { get; set; }
            public string PutUrl => $"test-url/get{_id}";
        }
        private class TestResponse
        {
            public string MyResponse { get; set; }
        }
    }
}