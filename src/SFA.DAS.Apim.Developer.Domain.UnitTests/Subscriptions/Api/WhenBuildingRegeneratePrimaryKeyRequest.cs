using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingRegeneratePrimaryKeyRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Built_And_Model_Added_To_Request(
            string subscriptionId)
        {
            subscriptionId += "+more things";
            var encodedSubscriptionId = HttpUtility.UrlEncode(subscriptionId);
            
            var actual = new RegeneratePrimaryKeyRequest(subscriptionId);

            actual.PostUrl.Should().Be($"subscriptions/{encodedSubscriptionId}/regeneratePrimaryKey?api-version=2023-09-01-preview");
            actual.Data.Should().NotBeNull();
        }
    }
}