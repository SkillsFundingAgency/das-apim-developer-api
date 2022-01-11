using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.UpdateUserState;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenHandlingUpdateUserCommand
    {
        [Test, MoqAutoData]
        public async Task Then_The_Command_Is_Handled_And_Service_Called(
            UpdateUserCommand command,
            UserDetails userDetails,
            [Frozen] Mock<IUserService> userService,
            UpdateUserCommandHandler handler)
        {
            userService.Setup(x => x.UpdateUser(
                It.Is<UserDetails>(c =>
                    c.Email.Equals(command.Email)
                    && c.Id.Equals(command.Id)
                    && c.FirstName.Equals(command.FirstName)
                    && c.LastName.Equals(command.LastName)
                    && c.State.Equals(command.State)
                    && c.Password.Equals(command.Password)
                    && c.Note.ConfirmEmailLink.Equals(command.ConfirmEmailLink)
                ))).ReturnsAsync(userDetails);
            
            var actual = await handler.Handle(command, CancellationToken.None);
            
            actual.UserDetails.Should().Be(userDetails);
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Command_Is_Handled_And_Service_Called_With_Null_UserNote(
            UpdateUserCommand command,
            UserDetails userDetails,
            [Frozen] Mock<IUserService> userService,
            UpdateUserCommandHandler handler)
        {
            command.ConfirmEmailLink = null;
            userService.Setup(x => x.UpdateUser(
                It.Is<UserDetails>(c =>
                    c.Email.Equals(command.Email)
                    && c.Id.Equals(command.Id)
                    && c.FirstName.Equals(command.FirstName)
                    && c.LastName.Equals(command.LastName)
                    && c.State.Equals(command.State)
                    && c.Password.Equals(command.Password)
                    && c.Note == null
                ))).ReturnsAsync(userDetails);
            
            var actual = await handler.Handle(command, CancellationToken.None);
            
            actual.UserDetails.Should().Be(userDetails);
        }
    }
}