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
using SFA.DAS.Apim.Developer.Api.Controllers;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.UpdateUserState;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Users
{
    public class WhenUpdatingAUserState
    {
        [Test, MoqAutoData]
        public async Task Then_The_Mediator_Command_Is_Called_And_No_Content_Result_Returned(
            string id,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            var controllerResult = await controller.UpdateUserState(id) as NoContentResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            mockMediator
                .Verify(mediator => mediator.Send(
                    It.Is<UpdateUserStateCommand>(c =>
                         c.UserId.Equals(id)
                    ),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_If_Error_An_Internal_Server_Error_Is_Returned(
            string id,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<UpdateUserStateCommand>(c =>
                        c.UserId.Equals(id)
                    ),
                    It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            
            var controllerResult = await controller.UpdateUserState(id) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}