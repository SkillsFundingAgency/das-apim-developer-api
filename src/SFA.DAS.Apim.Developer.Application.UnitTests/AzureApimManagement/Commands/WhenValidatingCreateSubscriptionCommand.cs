using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateSubscription;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenValidatingCreateSubscriptionCommand
    {
        [Test, MoqAutoData]
        public async Task Then_If_All_Fields_Set_Then_Valid(
            CreateSubscriptionCommand command,
            CreateSubscriptionCommandValidator validator)
        {
            var actual = await validator.ValidateAsync(command);

            actual.IsValid().Should().BeTrue();
        }
        
        [Test]
        [MoqInlineAutoData((string)null)]
        [MoqInlineAutoData("")]
        public async Task Then_If_No_InternalUserId_Set_Then_Error(
            string internalUserId,
            CreateSubscriptionCommand command,
            CreateSubscriptionCommandValidator validator)
        {
            command.InternalUserId = internalUserId;

            var actual = await validator.ValidateAsync(command);
            
            actual.IsValid().Should().BeFalse();
            actual.ValidationDictionary.Count.Should().Be(1);
            actual.ValidationDictionary.Should().ContainKey(nameof(command.InternalUserId));
        }
        
        
        [Test]
        [MoqInlineAutoData((string)null)]
        [MoqInlineAutoData("")]
        public async Task Then_If_No_ProductName_Set_Then_Error(
            string productName,
            CreateSubscriptionCommand command,
            CreateSubscriptionCommandValidator validator)
        {
            command.ProductName = productName;
            
            var actual = await validator.ValidateAsync(command);
            
            actual.IsValid().Should().BeFalse();
            actual.ValidationDictionary.Count.Should().Be(1);
            actual.ValidationDictionary.Should().ContainKey(nameof(command.ProductName));
        }
    }
}