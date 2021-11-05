using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingCreateSubscriptionRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Built_And_Model_Added_To_Request(
            string subscriptionId,
            string internalUserRef,
            string apimUserId,
            string productId)
        {
            var actual = new CreateSubscriptionRequest(subscriptionId, apimUserId, productId);

            actual.PutUrl.Should().Be($"subscriptions/{subscriptionId}?api-version=2021-04-01-preview");
            var actualData = (CreateSubscriptionRequestBody)actual.Data;
            actualData.Properties.DisplayName.Should().Be(subscriptionId);
            actualData.Properties.OwnerId.Should().Be($"/users/{apimUserId}");
            actualData.Properties.Scope.Should().Be($"/products/{productId}");
            actualData.Properties.State.Should().Be(SubscriptionState.Active);
        }
    }
}