using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Products.Api
{
    public class WhenBuildingGetProductRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string id)
        {
            id += "+more things";
            var encodedId = HttpUtility.UrlEncode(id);
            
            var actual = new GetProductRequest(id);

            actual.GetUrl.Should().Be($"products/{encodedId}?api-version=2021-08-01");
        }
    }
}