using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingGetUserSubscriptionSecretsRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string subscriptionId)
        {
            subscriptionId += "+more things";
            var encodedSubscriptionId = HttpUtility.UrlEncode(subscriptionId);
            
            var actual = new GetUserSubscriptionSecretsRequest(subscriptionId);

            actual.PostUrl.Should().Be($"subscriptions/{encodedSubscriptionId}/listSecrets?api-version=2023-09-01-preview");
        }
    }
}