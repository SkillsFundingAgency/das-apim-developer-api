using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenGettingProducts
    {
        [Test, MoqAutoData]
        public async Task Then_The_Products_Are_Returned_From_The_Api_And_Filtered_By_Group(
            GetProductsResponse apiResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            ProductService service)
        {
            //Arrange
            var expectedResponse = new List<Product>
            {
                new Product
                {
                    Name =apiResponse.Value.First().Name, 
                },
                new Product
                {
                    Name = apiResponse.Value.Last().Name, 
                }
            };
            azureApimManagementService.Setup(x => x.Get<GetProductsResponse>(It.IsAny<GetProductsRequest>()))
                .ReturnsAsync(new ApiResponse<GetProductsResponse>(apiResponse, HttpStatusCode.OK, ""));
            
            //Act
            var actual = await service.GetProducts(new List<string>
            {
                apiResponse.Value.First().Properties.Groups.First().Name, 
                apiResponse.Value.Last().Properties.Groups.Last().Name
            });

            //Assert
            actual.Should().BeEquivalentTo(expectedResponse);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Error_Then_Empty_List_Returned(
            GetProductsResponse apiResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            ProductService service)
        {
            //Arrange
            azureApimManagementService.Setup(x => x.Get<GetProductsResponse>(It.IsAny<GetProductsRequest>()))
                .ReturnsAsync(new ApiResponse<GetProductsResponse>(null, HttpStatusCode.NotFound, ""));
            
            //Act
            var actual = await service.GetProducts(new List<string>
            {
                apiResponse.Value.First().Properties.Groups.First().Name, 
                apiResponse.Value.Last().Properties.Groups.Last().Name
            });

            //Assert
            actual.Should().BeEmpty();
        }
    }
}