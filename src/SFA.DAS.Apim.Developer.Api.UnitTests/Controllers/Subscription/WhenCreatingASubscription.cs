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
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Subscription
{
    public class WhenCreatingASubscription
    {
        [Test, MoqAutoData]
        public async Task Then_The_Request_Is_Sent_To_The_Mediator_Handler(
            CreateSubscriptionApiRequest request,
            CreateUserSubscriptionCommandResponse mediatorResponse,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] SubscriptionController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<CreateUserSubscriptionCommand>(c =>
                        c.ProductName.Equals(request.ProductId)
                        && c.InternalUserId.Equals(request.AccountIdentifier)),
                    It.IsAny<CancellationToken>())).ReturnsAsync(mediatorResponse);

            var controllerResult = await controller.CreateSubscription(request) as CreatedResult;

            controllerResult!.StatusCode.Should().Be((int) HttpStatusCode.Created);
            controllerResult.Value.Should().BeEquivalentTo(new{Id=mediatorResponse.SubscriptionId});
        }
        
        [Test, MoqAutoData]
        public async Task And_Validation_Exception_Then_Returns_BadRequest(
            string errorKey,
            CreateSubscriptionApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] SubscriptionController controller)
        {
            var validationResult = new ValidationResult{ValidationDictionary = { {errorKey,"Some error"}}};
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<CreateUserSubscriptionCommand>(),
                    It.IsAny<CancellationToken>()))
                .Throws(new ValidationException(validationResult.DataAnnotationResult, null, null));

            var controllerResult = await controller.CreateSubscription(request) as ObjectResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            controllerResult.Value.ToString().Should().Contain(errorKey);
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_Exception_Then_Returns_Error(
            CreateSubscriptionApiRequest request,
            [Frozen] Mock<IMediator> mockMediator,
            [Greedy] SubscriptionController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<CreateUserSubscriptionCommand>(),
                    It.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();

            var controllerResult = await controller.CreateSubscription(request) as StatusCodeResult;

            controllerResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}