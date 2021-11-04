using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateSubscription;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.ApiResponses
{
    public class WhenCastingToCreateSubscriptionApiResponse
    {
        [Test, AutoData]
        public void Then_Maps_Fields(CreateSubscriptionCommandResponse source)
        {
            CreateSubscriptionApiResponse result = source;

            result.LiveSubscription.Should().NotBeNull();
            result.LiveSubscription.PrimaryKey.Should().Be(source.PrimaryKey);
            result.SandboxSubscription.Should().NotBeNull();
            result.SandboxSubscription.PrimaryKey.Should().Be(source.SandboxPrimaryKey);
        }
    }
}