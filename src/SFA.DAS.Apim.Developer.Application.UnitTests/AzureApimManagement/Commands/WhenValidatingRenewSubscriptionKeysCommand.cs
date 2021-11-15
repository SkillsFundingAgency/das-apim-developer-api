using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.RenewSubscriptionKeys;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenValidatingRenewSubscriptionKeysCommand
    {
        [Test, MoqAutoData]
        public async Task And_All_Fields_Set_Then_Valid(
            RenewSubscriptionKeysCommand command,
            RenewSubscriptionKeysCommandValidator validator)
        {
            var actual = await validator.ValidateAsync(command);

            actual.IsValid().Should().BeTrue();
        }
        
        [Test]
        [MoqInlineAutoData((string)null)]
        [MoqInlineAutoData("")]
        public async Task And_No_InternalUserId_Set_Then_Error(
            string internalUserId,
            RenewSubscriptionKeysCommand command,
            RenewSubscriptionKeysCommandValidator validator)
        {
            command.InternalUserId = internalUserId;

            var actual = await validator.ValidateAsync(command);
            
            actual.IsValid().Should().BeFalse();
            actual.ValidationDictionary.Count.Should().Be(1);
            actual.ValidationDictionary.Should().ContainKey(nameof(command.InternalUserId));
        }
    }
}