using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingGetUserSubscriptionsRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string name)
        {
            name += "+more things";
            var encodedName = HttpUtility.UrlEncode(name);
            
            var actual = new GetUserSubscriptionsRequest(name);

            actual.GetUrl.Should().Be($"subscriptions?$filter=startswith(name,'{encodedName}')&api-version=2023-09-01-preview");
        }
    }
}