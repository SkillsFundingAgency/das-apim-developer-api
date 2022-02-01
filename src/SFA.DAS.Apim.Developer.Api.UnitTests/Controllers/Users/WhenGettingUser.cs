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
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserByEmail;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Users
{
    public class WhenGettingUser
    {
        [Test, MoqAutoData]
        public async Task Then_The_Mediator_Query_Is_Called_And_User_Returned(
            string email,
            GetUserByEmailQueryResponse response,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetUserByEmailQuery>(c =>
                        c.Email.Equals(email)
                    ),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var controllerResult = await controller.GetUser(email) as OkObjectResult;

            controllerResult.Should().NotBeNull();
            var actualModel = controllerResult.Value as GetUserApiResponse;
            actualModel.Should().BeEquivalentTo((GetUserApiResponse)response.User);
        }
        
        [Test, MoqAutoData]
        public async Task And_Null_From_Mediator_Then_NotFound_Returned(
            string email,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<GetUserByEmailQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetUserByEmailQueryResponse());
            
            var controllerResult = await controller.GetUser(email) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
        
        [Test, MoqAutoData]
        public async Task And_Error_Then_InternalServerError_Is_Returned(
            string email,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<GetUserByEmailQuery>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
            
            var controllerResult = await controller.GetUser(email) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}