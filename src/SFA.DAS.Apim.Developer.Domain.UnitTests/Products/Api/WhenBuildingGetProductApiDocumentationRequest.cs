using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Products.Api
{
    public class WhenBuildingGetProductApiDocumentationRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string apiName)
        {
            apiName += "+more things";
            var encodedApiName = HttpUtility.UrlEncode(apiName);
            
            var actual = new GetProductApiDocumentationRequest(apiName);

            actual.GetUrl.Should().Be($"apis/{encodedApiName}?api-version=2023-09-01-preview");
        }
    }
}