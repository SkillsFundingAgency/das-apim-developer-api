using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
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
            ApiResponse<ApimUserResponseItem> apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponseItem>(It.Is<GetApimUserByIdRequest>(c =>
                    c.GetUrl.Contains($"{id}")), "application/json")).ReturnsAsync(apimUserResponse);
            
            var actual = await userService.GetUserById(id);
            
            actual.Should().BeEquivalentTo(apimUserResponse.Body.Properties);
            actual.Id.Should().Be(apimUserResponse.Body.Name);
        }
    }
}