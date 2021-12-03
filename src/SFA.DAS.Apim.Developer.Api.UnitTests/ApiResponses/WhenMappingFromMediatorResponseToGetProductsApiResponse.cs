using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
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
        
        [Test, AutoData]
        public void Then_Adds_Sandbox_To_DisplayName_If_In_Id(GetProductsQueryResponse source)
        {
            foreach (var product in source.Products)
            {
                product.Id = product + "-Sandbox";
            }
            var actual = (GetProductsApiResponse)source;
            
            actual.Products.Should().BeEquivalentTo(source.Products, options => 
                options.Excluding(product => product.DisplayName));
            actual.Products.Select(item => item.DisplayName).Should().BeEquivalentTo(
                source.Products.Select(product => product.DisplayName + " Sandbox"));
        }
    }
}
