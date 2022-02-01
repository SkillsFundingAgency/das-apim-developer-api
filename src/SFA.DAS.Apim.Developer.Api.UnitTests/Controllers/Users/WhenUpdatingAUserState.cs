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
            string id,
            UpsertUserApiRequest request,
            UpdateUserCommandResponse mediatorResponse,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<UpdateUserCommand>(c =>
                        c.Email.Equals(request.Email)
                        && c.Id.Equals(id)
                        && c.FirstName.Equals(request.FirstName)
                        && c.LastName.Equals(request.LastName)
                        && c.State.Equals(request.State.ToString())
                        && c.Password.Equals(request.Password)
                        && c.ConfirmEmailLink.Equals(request.ConfirmEmailLink)
                    ),
                    It.IsAny<CancellationToken>())).ReturnsAsync(mediatorResponse);
            
            var controllerResult = await controller.UpsertUser(id, request) as NoContentResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Not_Found_If_Null_From_Handler(
            string id,
            UpsertUserApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<UpdateUserCommand>(c =>
                        c.Email.Equals(request.Email)
                        && c.Id.Equals(id)
                        && c.FirstName.Equals(request.FirstName)
                        && c.LastName.Equals(request.LastName)
                        && c.State.Equals(request.State.ToString())
                        && c.Password.Equals(request.Password)
                        && c.ConfirmEmailLink.Equals(request.ConfirmEmailLink)
                    ),
                    It.IsAny<CancellationToken>())).ReturnsAsync(new UpdateUserCommandResponse
                {
                    UserDetails = null
                });
            
            var controllerResult = await controller.UpsertUser(id, request) as NotFoundResult;
            
            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Test, MoqAutoData]
        public async Task And_Validation_Exception_Then_Returns_BadRequest(
            string errorKey,
            string id,
            UpsertUserApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            var validationResult = new ValidationResult { ValidationDictionary = { { errorKey, "Some error" } } };
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<UpdateUserCommand>(c =>
                        c.Email.Equals(request.Email)
                    ),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationResult.DataAnnotationResult, null, null));

            var controllerResult = await controller.UpsertUser(id, request) as ObjectResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            controllerResult.Value.ToString().Should().Contain(errorKey);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Error_An_Internal_Server_Error_Is_Returned(
            string id,
            UpsertUserApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<UpdateUserCommand>(c =>
                        c.Email.Equals(request.Email)
                    ),
                    It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());
            
            var controllerResult = await controller.UpsertUser(id, request) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}