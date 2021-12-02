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
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserAuthenticated;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Users
{
    public class WhenCheckingUserAuthentication
    {
        [Test, MoqAutoData]
        public async Task Then_The_Mediator_Query_Is_Called_And_User_Returned(
            string email,
            string password,
            GetUserAuthenticatedQueryResponse response,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetUserAuthenticatedQuery>(c =>
                        c.Email.Equals(email)
                        && c.Password.Equals(password)
                    ),
                    It.IsAny<CancellationToken>())).ReturnsAsync(response);

            var controllerResult = await controller.AuthenticateUser(email, password) as OkObjectResult;

            controllerResult.Should().NotBeNull();
            var actualModel = controllerResult.Value as GetUserApiResponse;
            actualModel.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public async Task Then_If_Null_Then_Not_Found_Returned(
            string email,
            string password,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetUserAuthenticatedQuery>(c =>
                        c.Email.Equals(email)
                        && c.Password.Equals(password)
                    ),
                    It.IsAny<CancellationToken>())).ReturnsAsync(new GetUserAuthenticatedQueryResponse
                {
                    User = null
                });
            
            var controllerResult = await controller.AuthenticateUser(email, password) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Error_An_Internal_Server_Error_Is_Returned(
            string email,
            string password,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetUserAuthenticatedQuery>(c =>
                        c.Email.Equals(email)
                        && c.Password.Equals(password)
                    ),
                    It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            
            var controllerResult = await controller.AuthenticateUser(email, password) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}