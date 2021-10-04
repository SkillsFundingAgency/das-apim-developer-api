using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingCreateSubscriptionRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Built_And_Model_Added_To_Request(
            string apimResourceId,
            string subscriptionId,
            string subscriberType,
            string internalUserRef,
            string apimUserId,
            string productId)
        {
            var actual = new CreateSubscriptionRequest(apimResourceId, subscriptionId, subscriberType, internalUserRef, apimUserId, productId);

            actual.PutUrl.Should().Be($"{apimResourceId}/subscriptions/{subscriptionId}?api-version=2021-04-01-preview");
            var actualData = (CreateSubscriptionRequestBody)actual.Data;
            actualData.Properties.DisplayName.Should().Be($"{subscriberType}-{internalUserRef}");
            actualData.Properties.OwnerId.Should().Be($"/users/{apimUserId}");
            actualData.Properties.Scope.Should().Be($"/products/{productId}");
            actualData.Properties.State.Should().Be(SubscriptionState.Active);
        }
    }
}