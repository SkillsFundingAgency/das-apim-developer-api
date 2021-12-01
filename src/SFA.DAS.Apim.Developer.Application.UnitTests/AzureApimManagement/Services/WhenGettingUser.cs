using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenGettingUser
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_The_Repository_Is_Called_And_User_Returned(
            string email,
            ApimUser user,
            ApiResponse<ApimUserResponse> apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json")).ReturnsAsync(apimUserResponse);
            
            var actual = await userService.GetUser(email);
            
            actual.Should().BeEquivalentTo(apimUserResponse.Body.Properties.First(), options=>options.Excluding(c=>c.Name));
            actual.Id.Should().Be(apimUserResponse.Body.Properties.First().Name);
        }
    }
}