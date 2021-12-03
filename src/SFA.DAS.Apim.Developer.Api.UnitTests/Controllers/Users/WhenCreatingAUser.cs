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
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUser;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Users
{
    public class WhenCreatingAUser
    {
        [Test, MoqAutoData]
        public async Task Then_The_Request_Is_Sent_To_The_Mediator_Handler(
            string response,
            UpsertUserApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<CreateUserCommand>(c =>
                        c.Email.Equals(request.Email)
                        && c.Password.Equals(request.Password)
                        && c.FirstName.Equals(request.FirstName)
                        && c.LastName.Equals(request.LastName)
                        && c.State.Equals(request.State.ToString())
                    ),
                    It.IsAny<CancellationToken>())).ReturnsAsync(response);
            
            var controllerResult = await controller.UpsertUser(request) as CreatedResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.Created);
            controllerResult.Value.Should().BeEquivalentTo(new {id = response});
            
        }

        [Test, MoqAutoData]
        public async Task And_Validation_Exception_Then_Returns_BadRequest(
            string errorKey,
            UpsertUserApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            var validationResult = new ValidationResult { ValidationDictionary = { { errorKey, "Some error" } } };
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<CreateUserCommand>(),
                    It.IsAny<CancellationToken>()))
                .Throws(new ValidationException(validationResult.DataAnnotationResult, null, null));

            var controllerResult = await controller.UpsertUser(request) as ObjectResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            controllerResult.Value.ToString().Should().Contain(errorKey);
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_Exception_Then_Returns_Error(
            UpsertUserApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] UsersController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<CreateUserCommand>(),
                    It.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();

            var controllerResult = await controller.UpsertUser(request) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}