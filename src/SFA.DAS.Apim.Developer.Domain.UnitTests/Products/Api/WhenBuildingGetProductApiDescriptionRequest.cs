using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Products.Api
{
    public class WhenBuildingGetProductApiDescriptionRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string apiName)
        {
            var actual = new GetProductApiDescriptionRequest(apiName);

            actual.GetUrl.Should().Be($"apis/{apiName}?api-version=2021-04-01-preview");
        }
    }
}