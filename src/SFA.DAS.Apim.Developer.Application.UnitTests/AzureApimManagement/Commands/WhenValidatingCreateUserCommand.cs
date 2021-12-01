using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUser;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenValidatingCreateUserCommand
    {
        [Test, AutoData]
        public async Task Then_Valid_If_All_Values_Supplied_Correctly(
            CreateUserCommand command, 
            CreateUserCommandValidator validator)
        {
            command.Email = $"{command.Email}@{command.Email}.com";

            var actual = await validator.ValidateAsync(command);

            actual.IsValid().Should().BeTrue();
        }
        
        [Test, AutoData]
        public async Task Then_Not_Valid_If_No_Email(
            CreateUserCommand command, 
            CreateUserCommandValidator validator)
        {
            command.Email = "";

            var actual = await validator.ValidateAsync(command);

            actual.IsValid().Should().BeFalse();
        }
        
        [Test, AutoData]
        public async Task Then_Not_Valid_If_Email_Is_Not_In_Correct_Format(
            CreateUserCommand command, 
            CreateUserCommandValidator validator)
        {
            var actual = await validator.ValidateAsync(command);

            actual.IsValid().Should().BeFalse();
        }

        [Test, AutoData]
        public async Task Then_Not_Valid_If_No_LastName(
            CreateUserCommand command, 
            CreateUserCommandValidator validator)
        {
            command.LastName = "";
            command.Email = $"{command.Email}@{command.Email}.com";

            var actual = await validator.ValidateAsync(command);

            actual.IsValid().Should().BeFalse();
        }
        
        [Test, AutoData]
        public async Task Then_Not_Valid_If_No_FirstName(
            CreateUserCommand command, 
            CreateUserCommandValidator validator)
        {
            command.FirstName = "";
            command.Email = $"{command.Email}@{command.Email}.com";

            var actual = await validator.ValidateAsync(command);

            actual.IsValid().Should().BeFalse();
        }
        
        [Test, AutoData]
        public async Task Then_Not_Valid_If_No_Password(
            CreateUserCommand command, 
            CreateUserCommandValidator validator)
        {
            command.Password = "";
            command.Email = $"{command.Email}@{command.Email}.com";

            var actual = await validator.ValidateAsync(command);

            actual.IsValid().Should().BeFalse();
        }
    }
}