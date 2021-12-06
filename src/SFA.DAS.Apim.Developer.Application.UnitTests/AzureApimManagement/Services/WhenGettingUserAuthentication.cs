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
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenGettingUserAuthentication
    {
        [Test, MoqAutoData]
        public async Task Then_The_Request_Is_Made_And_The_State_Returned(
            string email,
            string password,
            ApiResponse<ApimUserResponse> apimUserResponse,
            GetUserAuthenticatedResponse apimAuthenticatedResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IAzureUserAuthenticationManagementService> azureUserAuthenticationManagementService,
            UserService userService)
        {
            var expectedAuthenticatedValue = new GetUserAuthenticatedRequest(email, password);
            azureUserAuthenticationManagementService.Setup(x =>
                x.GetAuthentication<GetUserAuthenticatedResponse>(It.Is<GetUserAuthenticatedRequest>(c =>
                    c.AuthorizationHeaderValue.Equals(expectedAuthenticatedValue.AuthorizationHeaderValue)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(apimAuthenticatedResponse, HttpStatusCode.OK, ""));
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json")).ReturnsAsync(apimUserResponse);

            var actual = await userService.CheckUserAuthentication(email, password);
            
            actual.Should().BeEquivalentTo(apimUserResponse.Body.Values.First().Properties);
            actual.Id.Should().Be(apimUserResponse.Body.Values.First().Name);
            actual.Authenticated.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Then_If_Not_Authenticated_User_Returned_And_Authenticated_False(
            string email,
            string password,
            ApiResponse<ApimUserResponse> apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IAzureUserAuthenticationManagementService> azureUserAuthenticationManagementService,
            UserService userService)
        {
            var expectedAuthenticatedValue = new GetUserAuthenticatedRequest(email, password);
            azureUserAuthenticationManagementService.Setup(x =>
                    x.GetAuthentication<GetUserAuthenticatedResponse>(It.Is<GetUserAuthenticatedRequest>(c =>
                        c.AuthorizationHeaderValue.Equals(expectedAuthenticatedValue.AuthorizationHeaderValue)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(new GetUserAuthenticatedResponse(), HttpStatusCode.Unauthorized, "unauthorized"));
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json")).ReturnsAsync(apimUserResponse);
            
            var actual = await userService.CheckUserAuthentication(email, password);

            actual.Should().BeEquivalentTo(apimUserResponse.Body.Values.First().Properties);
            actual.Id.Should().Be(apimUserResponse.Body.Values.First().Name);
            actual.Authenticated.Should().BeFalse();
        }

        [Test, MoqAutoData]
        public async Task Then_If_User_Does_Not_Exist_Null_Returned(
            string email,
            string password,
            ApiResponse<ApimUserResponse> apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IAzureUserAuthenticationManagementService> azureUserAuthenticationManagementService,
            UserService userService)
        {
            var expectedAuthenticatedValue = new GetUserAuthenticatedRequest(email, password);
            azureUserAuthenticationManagementService.Setup(x =>
                    x.GetAuthentication<GetUserAuthenticatedResponse>(It.Is<GetUserAuthenticatedRequest>(c =>
                        c.AuthorizationHeaderValue.Equals(expectedAuthenticatedValue.AuthorizationHeaderValue)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(new GetUserAuthenticatedResponse(), HttpStatusCode.Unauthorized, "unauthorized"));
            apimUserResponse.Body.Count = 0;
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json")).ReturnsAsync(apimUserResponse);
            
            var actual = await userService.CheckUserAuthentication(email, password);

            actual.Should().BeNull();
        }
    }
}