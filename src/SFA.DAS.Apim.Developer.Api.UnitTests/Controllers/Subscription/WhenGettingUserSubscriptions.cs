using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Api.Controllers;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserSubscriptions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Subscription
{
    public class WhenGettingUserSubscriptions
    {
        [Test, MoqAutoData]
        public async Task Then_The_Data_Is_Returned_From_The_Handler(
            string userId,
            GetUserSubscriptionsQueryResponse response,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] SubscriptionController controller)
        {
            mediator.Setup(x => x.Send(It.Is<GetUserSubscriptionsQuery>(c => c.InternalUserId == userId),
                CancellationToken.None)).ReturnsAsync(response);

            var actual = await controller.GetUserSubscriptions(userId) as OkObjectResult;

            Assert.IsNotNull(actual);
            actual.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var actualModel = actual.Value as GetUserSubscriptionsApiResponse;
            Assert.IsNotNull(actual);
            actualModel.Should().BeEquivalentTo((GetUserSubscriptionsApiResponse)response);
        }

        [Test, MoqAutoData]
        public async Task Then_If_Error_Then_Internal_Server_Error_Returned(
            string userId,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] SubscriptionController controller)
        {
            mediator.Setup(x => x.Send(It.IsAny<GetUserSubscriptionsQuery>(),
                CancellationToken.None)).ThrowsAsync(new Exception());

            var actual = await controller.GetUserSubscriptions(userId) as StatusCodeResult;

            actual.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}