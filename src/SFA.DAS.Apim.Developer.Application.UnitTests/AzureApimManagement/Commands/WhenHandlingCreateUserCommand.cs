using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
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
            UserDetails userDetails,
            CreateUserCommand command,
            [Frozen] Mock<IValidator<CreateUserCommand>> validator,
            [Frozen] Mock<IUserService> userService,
            CreateUserCommandHandler handler)
        {
            validator.Setup(x => x.ValidateAsync(command)).ReturnsAsync(new ValidationResult
            {
                ValidationDictionary = {  }
            });
            userService.Setup(x => x.UpsertUser(
                It.Is<UserDetails>(c =>
                    c.Email.Equals(command.Email)
                    && c.Id.Equals(command.Id)
                    && c.Password.Equals(command.Password)
                    && c.FirstName.Equals(command.FirstName)
                    && c.LastName.Equals(command.LastName)
                    && c.State.Equals(command.State)
                ))).ReturnsAsync(userDetails);
            
            var actual = await handler.Handle(command, CancellationToken.None);
            
            actual.Should().Be(userDetails.Id);
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