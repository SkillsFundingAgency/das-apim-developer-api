using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUser;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Apim.Developer.Domain.Validation.ValidationResult;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenHandlingCreateUserCommand
    {
        [Test, MoqAutoData]
        public async Task Then_The_Command_Is_Handled_And_Validated_And_UserService_Create_Method_Called_If_Valid(
            CreateUserCommand command,
            [Frozen] Mock<IValidator<CreateUserCommand>> validator,
            [Frozen] Mock<IUserService> userService,
            CreateUserCommandHandler handler)
        {
            validator.Setup(x => x.ValidateAsync(command)).ReturnsAsync(new ValidationResult
            {
                ValidationDictionary = {  }
            });
            
            await handler.Handle(command, CancellationToken.None);
            
            userService.Verify(x=>x.CreateUser( 
                It.Is<UserDetails>(c=>
                    c.Email.Equals(command.Email)
                    && c.Password.Equals(command.Password)
                    && c.FirstName.Equals(command.FirstName)
                    && c.LastName.Equals(command.LastName)
                    )), Times.Once);
        }

        [Test, MoqAutoData]
        public void Then_If_Not_Valid_Then_ValidationException_Thrown(
            CreateUserCommand command,
            [Frozen] Mock<IValidator<CreateUserCommand>> validator,
            [Frozen] Mock<IUserService> userService,
            CreateUserCommandHandler handler)
        {
            validator.Setup(x => x.ValidateAsync(command)).ReturnsAsync(new ValidationResult
            {
                ValidationDictionary = { {"",""} }
            });

            Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}