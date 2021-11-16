using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.Controllers;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.RenewSubscriptionKeys;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Subscription
{
    public class WhenRenewingSubscriptionKeys
    {
        [Test, MoqAutoData]
        public async Task Then_The_Request_Is_Sent_To_The_Mediator_Handler(
            string id,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] SubscriptionController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<RenewSubscriptionKeysCommand>(c =>
                        c.InternalUserId.Equals(id)),
                    It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            var controllerResult = await controller.RenewSubscriptionKeys(id) as IStatusCodeActionResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test, MoqAutoData]
        public async Task And_Validation_Exception_Then_Returns_BadRequest(
            string errorKey,
            string id,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] SubscriptionController controller)
        {
            var validationResult = new ValidationResult { ValidationDictionary = { { errorKey, "Some error" } } };
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<RenewSubscriptionKeysCommand>(),
                    It.IsAny<CancellationToken>()))
                .Throws(new ValidationException(validationResult.DataAnnotationResult, null, null));

            var controllerResult = await controller.RenewSubscriptionKeys(id) as ObjectResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            controllerResult.Value.ToString().Should().Contain(errorKey);
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_Exception_Then_Returns_Error(
            string id,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] SubscriptionController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<RenewSubscriptionKeysCommand>(),
                    It.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();

            var controllerResult = await controller.RenewSubscriptionKeys(id) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}