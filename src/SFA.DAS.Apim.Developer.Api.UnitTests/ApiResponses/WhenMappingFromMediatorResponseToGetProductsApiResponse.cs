using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetProducts;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.ApiResponses
{
    public class WhenMappingFromMediatorResponseToGetProductsApiResponse
    {
        [Test, AutoData]
        public void Then_The_Fields_Are_Mapped(GetProductsQueryResponse source)
        {
            var actual = (GetProductsApiResponse)source;
            
            actual.Products.Should().BeEquivalentTo(source.Products);
        }
    }
}