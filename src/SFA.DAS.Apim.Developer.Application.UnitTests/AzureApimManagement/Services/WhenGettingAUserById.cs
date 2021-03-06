using System.Net;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenGettingAUserById
    {
        [Test, MoqAutoData]
        public async Task Then_The_Api_Is_Called_And_User_Returned(
            string id,
            ApimUserResponseItem apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            UserService userService)
        {
            apimUserResponse.Properties.Note =
                JsonConvert.SerializeObject(new UserNote {ConfirmEmailLink = apimUserResponse.Properties.Note});
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponseItem>(It.Is<GetApimUserByIdRequest>(c =>
                    c.GetUrl.Contains($"{id}")), "application/json"))
                .ReturnsAsync(new ApiResponse<ApimUserResponseItem>(apimUserResponse, HttpStatusCode.OK, ""));
            
            var actual = await userService.GetUserById(id);
            
            actual.Should().BeEquivalentTo(apimUserResponse.Properties, options => 
                options.Excluding(properties => properties.Note));
            actual.Note.Should().BeEquivalentTo(JsonConvert.DeserializeObject<UserNote>(apimUserResponse.Properties.Note));
            actual.Id.Should().Be(apimUserResponse.Name);
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Api_Is_Called_And_User_Returned_And_Non_Json_Note_Parsed(
            string id,
            ApimUserResponseItem apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                    x.Get<ApimUserResponseItem>(It.Is<GetApimUserByIdRequest>(c =>
                        c.GetUrl.Contains($"{id}")), "application/json"))
                .ReturnsAsync(new ApiResponse<ApimUserResponseItem>(apimUserResponse, HttpStatusCode.OK, ""));
            
            var actual = await userService.GetUserById(id);
            
            actual.Should().BeEquivalentTo(apimUserResponse.Properties, options => 
                options.Excluding(properties => properties.Note));
            actual.Note.ConfirmEmailLink.Should().BeEquivalentTo(apimUserResponse.Properties.Note);
            actual.Id.Should().Be(apimUserResponse.Name);
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Api_Is_Called_And_Null_Returned_If_Not_Exists(
            string id,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponseItem>(It.Is<GetApimUserByIdRequest>(c =>
                    c.GetUrl.Contains($"{id}")), "application/json")).ReturnsAsync(new ApiResponse<ApimUserResponseItem>(null, HttpStatusCode.NotFound, ""));
            
            var actual = await userService.GetUserById(id);

            actual.Should().BeNull();
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Api_Is_Called_And_Null_Returned_If_Error(
            string id,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponseItem>(It.Is<GetApimUserByIdRequest>(c =>
                    c.GetUrl.Contains($"{id}")), "application/json")).ReturnsAsync(new ApiResponse<ApimUserResponseItem>(null, HttpStatusCode.InternalServerError, ""));
            
            var actual = await userService.GetUserById(id);

            actual.Should().BeNull();
        }
    }
}