using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.UpdateUserState;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenHandlingUpdateUserStateCommand
    {
        [Test, MoqAutoData]
        public async Task Then_The_Command_Is_Handled_And_Service_Called(
            UpdateUserStateCommand command,
            [Frozen] Mock<IUserService> userService,
            UpdateUserStateCommandHandler handler)
        {
            await handler.Handle(command, CancellationToken.None);
            
            userService.Verify(x=>x.UpdateUserState(command.UserId), Times.Once);
        }
    }
}