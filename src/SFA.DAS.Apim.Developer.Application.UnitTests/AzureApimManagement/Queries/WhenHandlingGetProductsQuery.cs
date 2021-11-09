using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Queries
{
    public class WhenHandlingGetProductsQuery
    {
        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_And_Data_Returned(
            GetProductsQuery query,
            List<Product> serviceResponse,
            [Frozen] Mock<IProductService> service,
            GetProductsQueryHandler handler)
        {
            service.Setup(x => x.GetProducts(query.Groups)).ReturnsAsync(serviceResponse);

            var actual = await handler.Handle(query, CancellationToken.None);

            actual.Products.Should().BeEquivalentTo(serviceResponse);
        }
    }
}