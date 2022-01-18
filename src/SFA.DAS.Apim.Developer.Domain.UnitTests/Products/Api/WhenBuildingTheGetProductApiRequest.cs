using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Products.Api
{
    public class WhenBuildingTheGetProductApiRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string name)
        {
            name += "+more things";
            var encodedName = HttpUtility.UrlEncode(name);
            
            var actual = new GetProductApiRequest(name);

            actual.GetUrl.Should().Be($"products/{encodedName}/Apis?api-version=2020-12-01");
        }
    }
}