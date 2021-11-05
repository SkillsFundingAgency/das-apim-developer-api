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
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses;
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
            AzureResource azureResourcesResponse)
        {

            //Arrange
            var tokenProvider = new Mock<IAzureTokenService>();
            tokenProvider.Setup(x => x.GetToken()).ReturnsAsync(authToken);

            var apimResourceConfiguration = new Mock<IOptions<AzureApimManagementConfiguration>>();
            var config = new AzureApimManagementConfiguration { ApimServiceName = apimServiceName };
            apimResourceConfiguration.Setup(x => x.Value).Returns(config);

            var apimResourceExpectedUrl = $"https://management.azure.com/{new ListAzureApimResourcesRequest(config.ApimServiceName).PostUrl}";

            azureResourcesResponse.Id = apimResourceId;
            var apimResourcesResponse = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new AzureResourcesResponse
                {
                    TotalRecords = 1,
                    AzureResources = new List<AzureResource> { azureResourcesResponse }
                })),
                StatusCode = HttpStatusCode.OK
            };

            var apimResourcesHttpMessageHandler = MessageHandler.SetupMessageHandlerMock(apimResourcesResponse, new Uri(apimResourceExpectedUrl), HttpMethod.Post);
            var client = new HttpClient(apimResourcesHttpMessageHandler.Object);
            var azureApimResourceService = new AzureApimResourceService(client, tokenProvider.Object, apimResourceConfiguration.Object);

            //Act
            var actualResult = await azureApimResourceService.GetResourceId();

            //Assert
            apimResourcesHttpMessageHandler.Protected()
                .Verify<Task<HttpResponseMessage>>(
                    "SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(c =>
                        c.Method.Equals(HttpMethod.Post)
                        && c.RequestUri.Equals(new Uri(apimResourceExpectedUrl))
                        && c.Headers.Authorization.Scheme.Equals("Bearer")
                        && c.Headers.Authorization.Parameter.Equals(authToken)),
                    ItExpr.IsAny<CancellationToken>()
                );

            actualResult.Should().Be(apimResourceId);
        }

        [Test, AutoData]
        public void Then_An_Exception_Is_Thrown_If_No_Apim_Resources_Are_Returned(
            string authToken,
            string azureSubscriptionId,
            string apimResourceId,
            string apimServiceName)
        {

            //Arrange
            var tokenProvider = new Mock<IAzureTokenService>();
            tokenProvider.Setup(x => x.GetToken()).ReturnsAsync(authToken);

            var apimResourceConfiguration = new Mock<IOptions<AzureApimManagementConfiguration>>();
            var config = new AzureApimManagementConfiguration { ApimServiceName = apimServiceName };
            apimResourceConfiguration.Setup(x => x.Value).Returns(config);

            var apimResourceExpectedUrl = $"https://management.azure.com/{new ListAzureApimResourcesRequest(config.ApimServiceName).PostUrl}";

            var apimResourcesResponse = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new AzureResourcesResponse
                {
                    AzureResources = new List<AzureResource> { }
                })),
                StatusCode = HttpStatusCode.OK
            };

            var apimResourcesHttpMessageHandler = MessageHandler.SetupMessageHandlerMock(apimResourcesResponse, new Uri(apimResourceExpectedUrl), HttpMethod.Post);
            var client = new HttpClient(apimResourcesHttpMessageHandler.Object);
            var azureApimResourceService = new AzureApimResourceService(client, tokenProvider.Object, apimResourceConfiguration.Object);

            //Act Assert
            Assert.ThrowsAsync<Exception>(async () => await azureApimResourceService.GetResourceId());
        }

        [Test, AutoData]
        public void Then_An_Exception_Is_Thrown_If_Too_Many_Apim_Resources_Are_Returned(
            string authToken,
            string azureSubscriptionId,
            string apimResourceId,
            string apimServiceName,
            AzureResource azureResource1,
            AzureResource azureResource2)
        {


            //Arrange
            var tokenProvider = new Mock<IAzureTokenService>();
            tokenProvider.Setup(x => x.GetToken()).ReturnsAsync(authToken);

            var apimResourceConfiguration = new Mock<IOptions<AzureApimManagementConfiguration>>();
            var config = new AzureApimManagementConfiguration { ApimServiceName = apimServiceName };
            apimResourceConfiguration.Setup(x => x.Value).Returns(config);

            var apimResourceExpectedUrl = $"https://management.azure.com/{new ListAzureApimResourcesRequest(config.ApimServiceName).PostUrl}";

            azureResource1.Id = apimResourceId;
            var apimResourcesResponse = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new AzureResourcesResponse
                {
                    TotalRecords = 2,
                    AzureResources = new List<AzureResource> { azureResource1, azureResource2 }
                })),
                StatusCode = HttpStatusCode.OK
            };

            var apimResourcesHttpMessageHandler = MessageHandler.SetupMessageHandlerMock(apimResourcesResponse, new Uri(apimResourceExpectedUrl), HttpMethod.Post);
            var client = new HttpClient(apimResourcesHttpMessageHandler.Object);
            var azureApimResourceService = new AzureApimResourceService(client, tokenProvider.Object, apimResourceConfiguration.Object);

            //Act Assert
            Assert.ThrowsAsync<Exception>(async () => await azureApimResourceService.GetResourceId());
        }
    }
}