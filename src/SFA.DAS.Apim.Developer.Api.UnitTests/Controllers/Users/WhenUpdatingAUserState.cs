using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiRequests;
using SFA.DAS.Apim.Developer.Api.Controllers;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.UpdateUserState;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Users
{
    public class WhenUpdatingAUserState
    {
        [Test, MoqAutoData]
        public async Task Then_The_Mediator_Command_Is_Called_And_No_Content_Result_Returned(
            UpdateUserStateApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            var controllerResult = await controller.UpdateUserState(request) as NoContentResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            mockMediator
                .Verify(mediator => mediator.Send(
                    It.Is<UpdateUserStateCommand>(c =>
                         c.UserEmail.Equals(request.Email)
                    ),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task And_Validation_Exception_Then_Returns_BadRequest(
            string errorKey,
            UpdateUserStateApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            var validationResult = new ValidationResult { ValidationDictionary = { { errorKey, "Some error" } } };
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<UpdateUserStateCommand>(c =>
                        c.UserEmail.Equals(request.Email)
                    ),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationResult.DataAnnotationResult, null, null));

            var controllerResult = await controller.UpdateUserState(request) as ObjectResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            controllerResult.Value.ToString().Should().Contain(errorKey);
        }
        [Test, MoqAutoData]
        public async Task Then_If_Error_An_Internal_Server_Error_Is_Returned(
            UpdateUserStateApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<UpdateUserStateCommand>(c =>
                        c.UserEmail.Equals(request.Email)
                    ),
                    It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            
            var controllerResult = await controller.UpdateUserState(request) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}