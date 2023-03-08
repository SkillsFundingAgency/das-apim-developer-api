using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.Controllers;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.DeleteSubscription;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Subscription
{
    public class WhenDeletingASubscription
    {
        [Test, MoqAutoData]
        public async Task Then_The_Request_Is_Sent_To_The_Mediator_Handler(
            string id,
            string productId,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] SubscriptionController controller)
        {
            id.Should().NotBeEmpty();
            productId.Should().NotBeEmpty();

            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<DeleteSubscriptionCommand>(c =>
                        c.InternalUserId.Equals(id)
                        && c.ProductName.Equals(productId)),
                    It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            var controllerResult = await controller.DeleteSubscription(id, productId) as IStatusCodeActionResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test, MoqAutoData]
        public async Task And_Validation_Exception_Then_Returns_BadRequest(
            string errorKey,
            string id,
            string productId,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] SubscriptionController controller)
        {
            errorKey.Should().NotBeEmpty();
            id.Should().NotBeEmpty();
            productId.Should().NotBeEmpty();

            var validationResult = new ValidationResult { ValidationDictionary = { { errorKey, "Some error" } } };
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<DeleteSubscriptionCommand>(),
                    It.IsAny<CancellationToken>()))
                .Throws(new ValidationException(validationResult.DataAnnotationResult, null, null));

            var controllerResult = await controller.DeleteSubscription(id, productId) as ObjectResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            controllerResult.Value.ToString().Should().Contain(errorKey);
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_Exception_Then_Returns_Error(
            string id,
            string productId,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] SubscriptionController controller)
        {
            id.Should().NotBeEmpty();
            productId.Should().NotBeEmpty();

            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<DeleteSubscriptionCommand>(),
                    It.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();

            var controllerResult = await controller.DeleteSubscription(id, productId) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}