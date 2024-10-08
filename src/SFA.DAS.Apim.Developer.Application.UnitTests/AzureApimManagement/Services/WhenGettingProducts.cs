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
            string documentationResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            ProductService service)
        {
            //Arrange
            var expectedResponse = new List<Product>
            {
                new Product
                {
                    Id = apiResponse.Value.Last().Name,
                    Name = apiProductTwo.Value.First().Name,
                    DisplayName = apiProductTwo.Value.First().Properties.DisplayName,
                    Description = apiProductTwo.Value.First().Properties.Description,
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
                .Setup(x => x.Get<object>(It.Is<GetProductApiDocumentationRequest>(c => c.GetUrl.Contains($"apis/{apiProductTwo.Value.First().Name}?api-version=2023-09-01-preview")),"application/vnd.oai.openapi+json"))
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