using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateSubscription;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenHandlingCreatingSubscriptionCommand
    {
        [Test, MoqAutoData]
        public void Then_The_Request_Is_Validated_And_If_Not_Valid_Exception_Thrown(
            CreateSubscriptionCommand request,
            [Frozen]Mock<IValidator<CreateSubscriptionCommand>> validator,
            CreateSubscriptionCommandHandler handler)
        {
            //Arrange
            validator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult {ValidationDictionary = { {"Error", "Some error"}}});
            
            //Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => handler.Handle(request, CancellationToken.None));
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_Request_Is_Valid_The_Service_Is_Called_And_Marked_As_Employer_If_Alphanumeric_Id(
            string subscriptionId,
            CreateSubscriptionCommand request,
            Subscription response,
            [Frozen]Mock<IValidator<CreateSubscriptionCommand>> validator,
            [Frozen]Mock<ISubscriptionService> service,
            CreateSubscriptionCommandHandler handler)
        {
            //Arrange
            validator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult( ));
            service
                .Setup(x => x.CreateSubscription(request.InternalUserId, Domain.Models.ApimUserType.Employer, request.ProductName))
                .ReturnsAsync(response);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            actual.SubscriptionId.Should().Be(response.Name);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_The_Request_Is_Valid_The_Service_Is_Called_And_Marked_As_External_If_Guid(
            string subscriptionId,
            Guid internalUserId,
            CreateSubscriptionCommand request,
            Subscription response,
            [Frozen]Mock<IValidator<CreateSubscriptionCommand>> validator,
            [Frozen]Mock<ISubscriptionService> service,
            CreateSubscriptionCommandHandler handler)
        {
            //Arrange
            request.InternalUserId = internalUserId.ToString();
            validator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult( ));
            service
                .Setup(x => x.CreateSubscription(request.InternalUserId, ApimUserType.External, request.ProductName))
                .ReturnsAsync(response);

            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            actual.SubscriptionId.Should().Be(response.Name);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_The_InternalUserId_Is_Numeric_Then_Marked_As_Provider(
            int id,
            string subscriptionId,
            CreateSubscriptionCommand request,
            Subscription response,
            [Frozen]Mock<IValidator<CreateSubscriptionCommand>> validator,
            [Frozen]Mock<ISubscriptionService> service,
            CreateSubscriptionCommandHandler handler)
        {
            request.InternalUserId = id.ToString();
            validator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult( ));
            service
                .Setup(x => x.CreateSubscription(request.InternalUserId, Domain.Models.ApimUserType.Provider, request.ProductName))
                .ReturnsAsync(response);
            
            //Act
            var actual = await handler.Handle(request, CancellationToken.None);

            //Assert
            actual.SubscriptionId.Should().Be(response.Name);
        }
    }
}