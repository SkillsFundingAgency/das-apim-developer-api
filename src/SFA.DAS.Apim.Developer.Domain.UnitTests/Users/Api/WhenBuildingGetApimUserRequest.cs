using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Users.Api
{
    public class WhenBuildingGetApimUserRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string email)
        {
            //Arrange
            email = $"{email}!@£$%£$%+15";
            
            //act
            var actual = new GetApimUserRequest(email);
            
            //assert
            actual.GetUrl.Should().Be($"users?$filter=email eq '{HttpUtility.UrlEncode(email)}'&api-version=2023-09-01-preview");
        }
    }
}