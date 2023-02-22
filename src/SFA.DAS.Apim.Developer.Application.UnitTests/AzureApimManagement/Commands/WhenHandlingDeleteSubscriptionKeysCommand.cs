using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.DeleteSubscription;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenHandlingDeleteSubscriptionKeysCommand
    {
        [Test, MoqAutoData]
        public void Then_The_Request_Is_Validated_And_If_Not_Valid_Exception_Thrown(
            DeleteSubscriptionCommand request,
            [Frozen] Mock<IValidator<DeleteSubscriptionCommand>> validator,
            DeleteSubscriptionCommandHandler handler)
        {
            //Arrange
            validator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = { { "Error", "Some error" } } });

            //Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => handler.Handle(request, CancellationToken.None));
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_InternalUserId_Is_Not_Numeric_Then_Marked_As_Employer_And_Service_Called(
            DeleteSubscriptionCommand request,
            [Frozen] Mock<IValidator<DeleteSubscriptionCommand>> mockValidator,
            [Frozen] Mock<ISubscriptionService> mockService,
            DeleteSubscriptionCommandHandler handler)
        {
            //Arrange
            mockValidator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult());

            //Act
            await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(service =>
                    service.DeleteSubscription(request.InternalUserId, ApimUserType.Employer, request.ProductName),
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_InternalUserId_Is_Numeric_Then_Marked_As_Provider_And_Service_Called(
            int id,
            DeleteSubscriptionCommand request,
            [Frozen] Mock<IValidator<DeleteSubscriptionCommand>> mockValidator,
            [Frozen] Mock<ISubscriptionService> mockService,
            DeleteSubscriptionCommandHandler handler)
        {
            request.InternalUserId = id.ToString();
            mockValidator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult());

            //Act
            await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(service =>
                    service.DeleteSubscription(request.InternalUserId, ApimUserType.Provider, request.ProductName),
                Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_InternalUserId_Is_Guid_Then_Marked_As_External_And_Service_Called(
            Guid id,
            DeleteSubscriptionCommand request,
            [Frozen] Mock<IValidator<DeleteSubscriptionCommand>> mockValidator,
            [Frozen] Mock<ISubscriptionService> mockService,
            DeleteSubscriptionCommandHandler handler)
        {
            request.InternalUserId = id.ToString();
            mockValidator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult());

            //Act
            await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(service =>
                    service.DeleteSubscription(request.InternalUserId, ApimUserType.External, request.ProductName),
                Times.Once);
        }
    }
}