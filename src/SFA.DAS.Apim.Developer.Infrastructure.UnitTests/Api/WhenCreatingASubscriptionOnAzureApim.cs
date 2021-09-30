using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Infrastructure.Api;

namespace SFA.DAS.Apim.Developer.Infrastructure.UnitTests.Api
{
    public class WhenCreatingASubscriptionOnAzureApim
    {
        [Test, AutoData]
        public async Task Then_The_Endpoint_Is_Called_And_Subscription_Created(
            string apiKey,
            string apimManagementUrl)
        {
            //Arrange
            apimManagementUrl = $"https://test.local/{apimManagementUrl}";
            var configuration = new Mock<IOptions<AzureApimManagementConfiguration>>();
            configuration.Setup(x => x.Value.Key).Returns(apiKey);
            configuration.Setup(x => x.Value.Url).Returns(apimManagementUrl);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject("")),
                StatusCode = HttpStatusCode.Accepted
            };
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(apimManagementUrl), HttpMethod.Post);
            var client = new HttpClient(httpMessageHandler.Object);
            var apprenticeshipService = new AzureApimManagementService(client, configuration.Object);
            
            //Act
            await apprenticeshipService.CreateSubscription();
            
            //Assert
            
        }
    }
}