using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenHandlingCreatingUserSubscriptionCommand
    {
        //TODO: test the validator
        // [Test, MoqAutoData]
        // public void Then_The_Request_Is_Validated_And_If_Not_Valid_Exception_Thrown(
        //     CreateUserSubscriptionCommand request,
        //     [Frozen]Mock<IValidator<CreateUserSubscriptionCommand>> validator,
        //     CreateUserSubscriptionCommandHandler handler)
        // {
        //     //Arrange
        //     validator
        //         .Setup(x => x.ValidateAsync(request))
        //         .ReturnsAsync(new ValidationResult {ValidationDictionary = { {"Error", "Some error"}}});
            
        //     //Act / Assert
        //     Assert.ThrowsAsync<ValidationException>(() => handler.Handle(request, CancellationToken.None));
        // }

        [Test, MoqAutoData]
        public async Task Then_If_The_Request_Is_Valid_The_Service_Is_Called(
            string subscriptionId,
            CreateUserSubscriptionCommand request,
            [Frozen]Mock<IValidator<CreateUserSubscriptionCommand>> validator,
            [Frozen]Mock<ISubscriptionService> service,
            CreateUserSubscriptionCommandHandler handler)
        {
            //Arrange
            validator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult( ));
            service.Setup(x => x.CreateUserSubscription(
                It.Is<string>( c => c.Equals(request.InternalUserId)),
                It.Is<string>(c => c.Equals(request.ApimUserType)),
                It.Is<string>(c => c.Equals(request.ProductName))
            )).ReturnsAsync(subscriptionId);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            actual.SubscriptionId.Should().Be(subscriptionId);
        }
    }
}