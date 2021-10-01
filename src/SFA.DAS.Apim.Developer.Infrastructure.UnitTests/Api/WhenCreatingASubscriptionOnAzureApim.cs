using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Azure.Services.AppAuthentication;
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
            string apimServiceName)
        {
            //TODO: fix

            //Arrange
            var configuration = new Mock<IOptions<AzureApimManagementConfiguration>>();
            var tokenProvider = new Mock<AzureServiceTokenProvider>();
            configuration.Setup(x => x.Value.ApimServiceName).Returns(apimServiceName);
            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject("")),
                StatusCode = HttpStatusCode.Accepted
            };
            var httpMessageHandler = MessageHandler.SetupMessageHandlerMock(response, new Uri(apimManagementUrl), HttpMethod.Post);
            var client = new HttpClient(httpMessageHandler.Object);
            var apprenticeshipService = new AzureApimManagementService(client, configuration.Object, tokenProvider.Object);

            //Act
            await apprenticeshipService.CreateSubscription();

            //Assert

        }
    }
}