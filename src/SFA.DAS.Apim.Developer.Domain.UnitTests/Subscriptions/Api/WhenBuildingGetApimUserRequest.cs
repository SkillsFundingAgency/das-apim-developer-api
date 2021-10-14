using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingGetApimUserRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string email)
        {
            //act
            var actual = new GetApimUserRequest(email);
            
            //assert
            actual.GetUrl.Should().Be($"users?$filter=email eq '{email}'&api-version=2021-04-01-preview");
        }
    }
}