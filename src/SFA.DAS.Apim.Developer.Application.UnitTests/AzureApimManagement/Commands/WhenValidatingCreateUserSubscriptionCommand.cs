using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Commands
{
    public class WhenValidatingCreateUserSubscriptionCommand
    {
        [Test, MoqAutoData]
        public async Task Then_If_All_Fields_Set_Then_Valid(
            CreateUserSubscriptionCommand command,
            CreateUserSubscriptionCommandValidator validator)
        {
            command.UserDetails.EmailAddress = $"{command.UserDetails.EmailAddress}@validemailaddress.com";
            var actual = await validator.ValidateAsync(command);

            actual.IsValid().Should().BeTrue();
        }
        
        [Test]
        [MoqInlineAutoData((string)null)]
        [MoqInlineAutoData("")]
        public async Task Then_If_No_InternalUserId_Set_Then_Error(
            string internalUserId,
            CreateUserSubscriptionCommand command,
            CreateUserSubscriptionCommandValidator validator)
        {
            command.InternalUserId = internalUserId;
            command.UserDetails.EmailAddress = $"{command.UserDetails.EmailAddress}@validemailaddress.com";
                
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
            CreateUserSubscriptionCommand command,
            CreateUserSubscriptionCommandValidator validator)
        {
            command.ProductName = productName;
            command.UserDetails.EmailAddress = $"{command.UserDetails.EmailAddress}@validemailaddress.com";
            
            var actual = await validator.ValidateAsync(command);
            
            actual.IsValid().Should().BeFalse();
            actual.ValidationDictionary.Count.Should().Be(1);
            actual.ValidationDictionary.Should().ContainKey(nameof(command.ProductName));
        }
    }
}