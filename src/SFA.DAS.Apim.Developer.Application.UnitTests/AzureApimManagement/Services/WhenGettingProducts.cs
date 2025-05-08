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
        public async Task Then_The_Products_Are_Returned_From_The_Api_And_Filtered_By_Group_Ignoring_Case_And_Skipping_Products_With_No_Apis(
            GetProductsResponse apiResponse,
            GetProductApisResponse apiProductOne,
            GetProductApisResponse apiProductTwo,
            GetProductApiItem apiProductTwoItem,
            string documentationResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            ProductService service)
        {
            //Arrange
            apiProductTwo.Value = [apiProductTwoItem];
            var expectedResponse = new List<Product>
            {
                new Product
                {
                    Id = apiResponse.Value.Last().Name,
                    Name = apiProductTwoItem.Name,
                    DisplayName = apiProductTwoItem.Properties.DisplayName,
                    Description = apiProductTwoItem.Properties.Description,
                    Documentation = documentationResponse
                }
            };
            apiProductOne.Count = 0;
            azureApimManagementService.Setup(x => x.Get<GetProductsResponse>(It.IsAny<GetProductsRequest>(), "application/json"))
                .ReturnsAsync(new ApiResponse<GetProductsResponse>(apiResponse, HttpStatusCode.OK, ""));
            azureApimManagementService
                .Setup(x => x.Get<GetProductApisResponse>(It.Is<GetProductApiRequest>(c => c.GetUrl.Contains($"products/{apiResponse.Value.First().Name}/Apis")), "application/json"))
                .ReturnsAsync(new ApiResponse<GetProductApisResponse>(apiProductOne, HttpStatusCode.OK, ""));
            azureApimManagementService
                .Setup(x => x.Get<GetProductApisResponse>(It.Is<GetProductApiRequest>(c => c.GetUrl.Contains($"products/{apiResponse.Value.Last().Name}/Apis")),"application/json"))
                .ReturnsAsync(new ApiResponse<GetProductApisResponse>(apiProductTwo, HttpStatusCode.OK, ""));
            azureApimManagementService
                .Setup(x => x.Get<object>(It.Is<GetProductApiDocumentationRequest>(c => c.GetUrl.Contains($"apis/{apiProductTwoItem.Name}?api-version=2023-09-01-preview")),"application/vnd.oai.openapi+json"))
                .ReturnsAsync(new ApiResponse<object>(documentationResponse, HttpStatusCode.OK, ""));
            
            //Act
            var actual = await service.GetProducts(new List<string>
            {
                apiResponse.Value.First().Properties.Groups.First().Name, 
                apiResponse.Value.Last().Properties.Groups.Last().Name.ToUpper()
            });

            //Assert
            actual.Should().BeEquivalentTo(expectedResponse);
        }

        [Test, MoqAutoData]
        public async Task Then_If_There_Are_Multiple_Api_Versions_They_Are_Returned(
            GetProductItem product,
            GetProductsResponse apiResponse,
            GetProductApisResponse apiProductTwo,
            GetProductApiItem apiProductTwoItem,
            GetProductApiItem apiProductTwoItem2,
            string documentationResponse,
            string documentationResponse2,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            ProductService service)
        {
            //Arrange
            apiProductTwo.Value = [apiProductTwoItem,apiProductTwoItem2];
            var expectedResponse = new List<Product>
            {
                new Product
                {
                    Id = product.Name,
                    Name = apiProductTwoItem.Name,
                    DisplayName = apiProductTwoItem.Properties.DisplayName,
                    Description = apiProductTwoItem.Properties.Description,
                    Documentation = documentationResponse
                },
                new Product
                {
                    Id = product.Name,
                    Name = apiProductTwoItem2.Name,
                    DisplayName = apiProductTwoItem2.Properties.DisplayName,
                    Description = apiProductTwoItem2.Properties.Description,
                    Documentation = documentationResponse2
                }
            };
            apiResponse.Value = [product];
            azureApimManagementService.Setup(x => x.Get<GetProductsResponse>(It.IsAny<GetProductsRequest>(), "application/json"))
                .ReturnsAsync(new ApiResponse<GetProductsResponse>(apiResponse, HttpStatusCode.OK, ""));
            azureApimManagementService
                .Setup(x => x.Get<GetProductApisResponse>(It.Is<GetProductApiRequest>(c => c.GetUrl.Contains($"products/{product.Name}/Apis")),"application/json"))
                .ReturnsAsync(new ApiResponse<GetProductApisResponse>(apiProductTwo, HttpStatusCode.OK, ""));
            azureApimManagementService
                .Setup(x => x.Get<object>(It.Is<GetProductApiDocumentationRequest>(c => c.GetUrl.Contains($"apis/{apiProductTwoItem.Name}?api-version=2023-09-01-preview")),"application/vnd.oai.openapi+json"))
                .ReturnsAsync(new ApiResponse<object>(documentationResponse, HttpStatusCode.OK, ""));
            azureApimManagementService
                .Setup(x => x.Get<object>(It.Is<GetProductApiDocumentationRequest>(c => c.GetUrl.Contains($"apis/{apiProductTwoItem2.Name}?api-version=2023-09-01-preview")),"application/vnd.oai.openapi+json"))
                .ReturnsAsync(new ApiResponse<object>(documentationResponse2, HttpStatusCode.OK, ""));
            
            //Act
            var actual = await service.GetProducts(new List<string>
            {
                apiResponse.Value.First().Properties.Groups.First().Name, 
                apiResponse.Value.Last().Properties.Groups.Last().Name.ToUpper()
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
            azureApimManagementService.Setup(x => x.Get<GetProductsResponse>(It.IsAny<GetProductsRequest>(), "application/json"))
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