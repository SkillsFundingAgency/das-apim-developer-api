using System.Linq;
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
    public class WhenGettingUser
    {
        [Test, MoqAutoData]
        public async Task Then_The_Api_Is_Called_And_User_Returned(
            string email,
            ApiResponse<ApimUserResponse> apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            UserService userService)
        {
            apimUserResponse.Body.Values.First().Properties.Note =
                JsonConvert.SerializeObject(new UserNote {ConfirmEmailLink = apimUserResponse.Body.Values.First().Properties.Note});
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json")).ReturnsAsync(apimUserResponse);
            
            var actual = await userService.GetUser(email);
            
            actual.Should().BeEquivalentTo(apimUserResponse.Body.Values.First().Properties, options => 
                options.Excluding(properties => properties.Note));
            actual.Note.Should().BeEquivalentTo(JsonConvert.DeserializeObject<UserNote>(apimUserResponse.Body.Values.First().Properties.Note));
            actual.Id.Should().Be(apimUserResponse.Body.Values.First().Name);
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Api_Is_Called_And_User_Returned_And_Non_Json_Note_Parsed(
            string email,
            ApiResponse<ApimUserResponse> apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json")).ReturnsAsync(apimUserResponse);
            
            var actual = await userService.GetUser(email);
            
            actual.Should().BeEquivalentTo(apimUserResponse.Body.Values.First().Properties, options => 
                options.Excluding(properties => properties.Note));
            actual.Note.ConfirmEmailLink.Should().Be(apimUserResponse.Body.Values.First().Properties.Note);
            actual.Id.Should().Be(apimUserResponse.Body.Values.First().Name);
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_The_Email_Is_Null_Then_Null_Returned(UserService userService)
        {
            var actual = await userService.GetUser(null);

            actual.Should().BeNull();
        }
    }
}