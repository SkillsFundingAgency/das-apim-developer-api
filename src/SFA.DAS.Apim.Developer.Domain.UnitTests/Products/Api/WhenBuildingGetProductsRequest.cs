using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Products.Api
{
    public class WhenBuildingGetProductsRequest
    {
        [Test]
        public void Then_The_Url_Is_Correctly_Constructed()
        {
            var actual = new GetProductsRequest();

            actual.GetUrl.Should().Be("products?expandGroups=true&api-version=2023-09-01-preview");
        }
    }
}