using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.RenewSubscriptionKeys;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenHandlingRenewSubscriptionKeysCommand
    {
        [Test, MoqAutoData]
        public void Then_The_Request_Is_Validated_And_If_Not_Valid_Exception_Thrown(
            RenewSubscriptionKeysCommand request,
            [Frozen]Mock<IValidator<RenewSubscriptionKeysCommand>> validator,
            RenewSubscriptionKeysCommandHandler handler)
        {
            //Arrange
            validator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult {ValidationDictionary = { {"Error", "Some error"}}});
            
            //Act / Assert
            Assert.ThrowsAsync<ValidationException>(() => handler.Handle(request, CancellationToken.None));
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_InternalUserId_Is_Not_Numeric_Then_Marked_As_Employer_And_Service_Called(
            string subscriptionId,
            RenewSubscriptionKeysCommand request,
            [Frozen]Mock<IValidator<RenewSubscriptionKeysCommand>> mockValidator,
            [Frozen]Mock<ISubscriptionService> mockService,
            RenewSubscriptionKeysCommandHandler handler)
        {
            //Arrange
            mockValidator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult( ));
            /*service
                .Setup(x => x.RegenerateSubscriptionKeys(request.InternalUserId, ApimUserType.Employer))
                .ReturnsAsync();*/

            //Act
            await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(service => 
                    service.RegenerateSubscriptionKeys(request.InternalUserId, ApimUserType.Employer), 
                Times.Once);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_The_InternalUserId_Is_Numeric_Then_Marked_As_Provider_And_Service_Called(
            int id,
            string subscriptionId,
            RenewSubscriptionKeysCommand request,
            Subscription response,
            [Frozen]Mock<IValidator<RenewSubscriptionKeysCommand>> mockValidator,
            [Frozen]Mock<ISubscriptionService> mockService,
            RenewSubscriptionKeysCommandHandler handler)
        {
            request.InternalUserId = id.ToString();
            mockValidator
                .Setup(x => x.ValidateAsync(request))
                .ReturnsAsync(new ValidationResult( ));
            /*service
                .Setup(x => x.RegenerateSubscriptionKeys(request.InternalUserId, Domain.Models.ApimUserType.Provider))
                .ReturnsAsync(Unit.Value);*/
            
            //Act
            await handler.Handle(request, CancellationToken.None);

            //Assert
            mockService.Verify(service => 
                    service.RegenerateSubscriptionKeys(request.InternalUserId, ApimUserType.Provider), 
                Times.Once);
        }
    }
}