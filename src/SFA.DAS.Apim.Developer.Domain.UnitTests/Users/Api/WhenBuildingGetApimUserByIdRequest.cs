using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Users.Api
{
    public class WhenBuildingGetApimUserByIdRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string id)
        {
            id += "+more things";
            var encodedId = HttpUtility.UrlEncode(id);
            
            //act
            var actual = new GetApimUserByIdRequest(id);
            
            //assert
            actual.GetUrl.Should().Be($"users/{encodedId}?api-version=2023-09-01-preview");
        }
    }
}