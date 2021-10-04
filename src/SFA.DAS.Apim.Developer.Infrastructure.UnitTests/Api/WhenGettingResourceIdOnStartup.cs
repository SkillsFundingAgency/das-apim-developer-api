using System;
using System.Collections.Generic;
using System.Linq;
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
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using SFA.DAS.Apim.Developer.Infrastructure.Api;
using SFA.DAS.Apim.Developer.Infrastructure.Models;

namespace SFA.DAS.Apim.Developer.Infrastructure.UnitTests.Api
{
    public class WhenGettingResourceIdOnStartup
    {
        [Test, AutoData]
        public async Task Then_The_Resource_Id_Is_Retrieved(
            string authToken,
            string azureSubscriptionId,
            string apimResourceId,
            string apimServiceName,
            AzureResourcesResponse.AzureResource azureResourcesResponse)
        {

            //Arrange
            var tokenProvider = new Mock<IAzureTokenService>();
            tokenProvider.Setup(x => x.GetToken()).ReturnsAsync(authToken);

            var apimResourceConfiguration = new Mock<IOptions<AzureApimManagementConfiguration>>();
            var config = new AzureApimManagementConfiguration { ApimServiceName = apimServiceName };
            apimResourceConfiguration.Setup(x => x.Value).Returns(config);

            var subsExpectedUrl = $"https://management.azure.com/{new GetAzureSubscriptionsRequest().GetUrl}";
            var apimResourceExpectedUrl = $"https://management.azure.com/{new GetAzureResourcesRequest(azureSubscriptionId, config.ApimServiceName).GetUrl}";

            var subsResponse = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new AzureSubscriptionsResponse
                {
                    value = new List<AzureSubscriptionsResponse.AzureSubscription>
                    {
                        new AzureSubscriptionsResponse.AzureSubscription{
                            displayName = "test",
                            id = $"/subscriptions/{azureSubscriptionId}",
                            subscriptionId = azureSubscriptionId
                        }
                    }
                })),
                StatusCode = HttpStatusCode.OK
            };
            
            azureResourcesResponse.id = apimResourceId;
            var apimResourcesResponse = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new AzureResourcesResponse
                {
                    value = new List<AzureResourcesResponse.AzureResource>{ azureResourcesResponse }
                })),
                StatusCode = HttpStatusCode.OK
            };

            var subsHttpMessageHandler = MessageHandler.SetupMessageHandlerMock(subsResponse, new Uri(subsExpectedUrl), HttpMethod.Get);
            var apimResourcesHttpMessageHandler = MessageHandler.SetupMessageHandlerMock(apimResourcesResponse, new Uri(apimResourceExpectedUrl), HttpMethod.Get, subsHttpMessageHandler);
            var client = new HttpClient(apimResourcesHttpMessageHandler.Object);
            var azureApimResourceService = new AzureApimResourceService(client, tokenProvider.Object, apimResourceConfiguration.Object);

            //Act
            var actualResult = await azureApimResourceService.GetResourceId();

            //Assert
            subsHttpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.AbsoluteUri.Equals(subsExpectedUrl)
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );

            apimResourcesHttpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Get)
                        && c.RequestUri.Equals(new Uri(apimResourceExpectedUrl))
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );

            actualResult.Should().Be(apimResourceId);
        }
    }
}