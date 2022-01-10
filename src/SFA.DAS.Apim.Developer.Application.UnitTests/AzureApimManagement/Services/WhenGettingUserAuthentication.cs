using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenGettingUserAuthentication
    {
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
            apimUserResponse.Body.Values.First().Properties.Note =
                JsonConvert.SerializeObject(new UserNote {ConfirmEmailLink = apimUserResponse.Body.Values.First().Properties.Note});
            var expectedAuthenticatedValue = new GetUserAuthenticatedRequest(email, password);
            azureUserAuthenticationManagementService.Setup(x =>
                x.GetAuthentication<GetUserAuthenticatedResponse>(It.Is<GetUserAuthenticatedRequest>(c =>
                    c.AuthorizationHeaderValue.Equals(expectedAuthenticatedValue.AuthorizationHeaderValue)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(apimAuthenticatedResponse, HttpStatusCode.OK, ""));
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json"))
                .ReturnsAsync(apimUserResponse);

            var actual = await userService.CheckUserAuthentication(email, password);
            
            actual.Should().BeEquivalentTo(apimUserResponse.Body.Values.First().Properties, options => 
                options.Excluding(properties => properties.Note));
            actual.Note.Should().BeEquivalentTo(JsonConvert.DeserializeObject<UserNote>(apimUserResponse.Body.Values.First().Properties.Note));
            actual.Id.Should().Be(apimUserResponse.Body.Values.First().Name);
            actual.Authenticated.Should().BeTrue();
        }
        
        [Test, MoqAutoData]
        public async Task Then_If_Not_Authenticated_User_Returned_And_Authenticated_False_And_FailCount_Incremented(
            string email,
            string password,
            ApiResponse<ApimUserResponse> apimUserResponse,
            UserResponse putUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IAzureUserAuthenticationManagementService> azureUserAuthenticationManagementService,
            UserService userService)
        {
            apimUserResponse.Body.Values.First().Properties.Note =
                JsonConvert.SerializeObject(new UserNote {ConfirmEmailLink = apimUserResponse.Body.Values.First().Properties.Note});
            var expectedAuthenticatedValue = new GetUserAuthenticatedRequest(email, password);
            azureUserAuthenticationManagementService.Setup(x =>
                x.GetAuthentication<GetUserAuthenticatedResponse>(It.Is<GetUserAuthenticatedRequest>(c =>
                    c.AuthorizationHeaderValue.Equals(expectedAuthenticatedValue.AuthorizationHeaderValue)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(new GetUserAuthenticatedResponse(), HttpStatusCode.Unauthorized, "unauthorized"));
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json"))
                .ReturnsAsync(apimUserResponse);
            azureApimManagementService.Setup(x =>
                x.Put<UserResponse>(It.Is<CreateUserRequest>(c =>
                    c.PutUrl.Contains($"{apimUserResponse.Body.Values[0].Name}"))))
                .ReturnsAsync(new ApiResponse<UserResponse>(putUserResponse, HttpStatusCode.OK, ""));
            
            var actual = await userService.CheckUserAuthentication(email, password);

            actual.Should().BeEquivalentTo(apimUserResponse.Body.Values.First().Properties, options => 
                options.Excluding(properties => properties.Note));
            actual.Note.FailedAuthCount.Should().Be(1);
            actual.Note.AccountLockedDateTime.Should().BeNull();
            actual.Id.Should().Be(apimUserResponse.Body.Values.First().Name);
            actual.Authenticated.Should().BeFalse();
            azureApimManagementService.Verify(service => service.Put<UserResponse>(It.Is<CreateUserRequest>(request => 
                ((CreateUserRequestBody)request.Data).Properties.Note.Contains(@"""FailedAuthCount"":1"))), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task And_Successful_Auth_After_Failed_Auth_Then_Resets_Count(
            string email,
            string password,
            ApiResponse<ApimUserResponse> apimUserResponse,
            UserResponse putUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IAzureUserAuthenticationManagementService> azureUserAuthenticationManagementService,
            UserService userService)
        {
            apimUserResponse.Body.Values.First().Properties.Note =
                JsonConvert.SerializeObject(new UserNote {ConfirmEmailLink = apimUserResponse.Body.Values.First().Properties.Note, FailedAuthCount = 2});
            var expectedAuthenticatedValue = new GetUserAuthenticatedRequest(email, password);
            azureUserAuthenticationManagementService.Setup(x =>
                x.GetAuthentication<GetUserAuthenticatedResponse>(It.Is<GetUserAuthenticatedRequest>(c =>
                    c.AuthorizationHeaderValue.Equals(expectedAuthenticatedValue.AuthorizationHeaderValue)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(new GetUserAuthenticatedResponse(), HttpStatusCode.OK, ""));
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json"))
                .ReturnsAsync(apimUserResponse);
            azureApimManagementService.Setup(x =>
                x.Put<UserResponse>(It.Is<CreateUserRequest>(c =>
                    c.PutUrl.Contains($"{apimUserResponse.Body.Values[0].Name}"))))
                .ReturnsAsync(new ApiResponse<UserResponse>(putUserResponse, HttpStatusCode.OK, ""));
            
            var actual = await userService.CheckUserAuthentication(email, password);

            actual.Should().BeEquivalentTo(apimUserResponse.Body.Values.First().Properties, options => 
                options.Excluding(properties => properties.Note));
            actual.Note.FailedAuthCount.Should().Be(0);
            actual.Note.AccountLockedDateTime.Should().BeNull();
            actual.Id.Should().Be(apimUserResponse.Body.Values.First().Name);
            actual.Authenticated.Should().BeTrue();
            azureApimManagementService.Verify(service => service.Put<UserResponse>(It.Is<CreateUserRequest>(request => 
                ((CreateUserRequestBody)request.Data).Properties.Note.Contains(@"""FailedAuthCount"":0"))), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task And_Fails_Authentication_And_Third_Fail_Then_Locks_Account(
            string email,
            string password,
            ApiResponse<ApimUserResponse> apimUserResponse,
            UserResponse putUserResponse,
            [Frozen] ApimDeveloperApiConfiguration config,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IAzureUserAuthenticationManagementService> azureUserAuthenticationManagementService,
            UserService userService)
        {
            config.NumberOfAuthFailuresToLockAccount = 3;
            apimUserResponse.Body.Values.First().Properties.Note =
                JsonConvert.SerializeObject(new UserNote {ConfirmEmailLink = apimUserResponse.Body.Values.First().Properties.Note, FailedAuthCount = 2});
            var expectedAuthenticatedValue = new GetUserAuthenticatedRequest(email, password);
            azureUserAuthenticationManagementService.Setup(x =>
                x.GetAuthentication<GetUserAuthenticatedResponse>(It.Is<GetUserAuthenticatedRequest>(c =>
                    c.AuthorizationHeaderValue.Equals(expectedAuthenticatedValue.AuthorizationHeaderValue)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(new GetUserAuthenticatedResponse(), HttpStatusCode.Unauthorized, "unauthorized"));
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json"))
                .ReturnsAsync(apimUserResponse);
            azureApimManagementService.Setup(x =>
                x.Put<UserResponse>(It.Is<CreateUserRequest>(c =>
                    c.PutUrl.Contains($"{apimUserResponse.Body.Values[0].Name}"))))
                .ReturnsAsync(new ApiResponse<UserResponse>(putUserResponse, HttpStatusCode.OK, ""));
            
            var actual = await userService.CheckUserAuthentication(email, password);

            actual.Should().BeEquivalentTo(apimUserResponse.Body.Values.First().Properties, options => options
                .Excluding(properties => properties.Note)
                .Excluding(properties => properties.State));
            actual.Note.FailedAuthCount.Should().Be(3);
            actual.Note.AccountLockedDateTime.Should().HaveValue().And.BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
            actual.Id.Should().Be(apimUserResponse.Body.Values.First().Name);
            actual.Authenticated.Should().BeFalse();
            actual.State.Should().Be("blocked");
            azureApimManagementService.Verify(service => service.Put<UserResponse>(It.Is<CreateUserRequest>(request => 
                ((CreateUserRequestBody)request.Data).Properties.Note.Contains(@$"""FailedAuthCount"":3,""AccountLockedDateTime"":""{DateTime.Now:yyyy-MM-dd}T")
                && ((CreateUserRequestBody)request.Data).Properties.State.Equals("blocked")))
                , Times.Once);
        }
        
        [Test, MoqAutoData]
        public async Task And_Authenticating_Inside_10min_Lock_Period_Then_Return_Locked(
            string email,
            string password,
            UserNote userNote,
            ApiResponse<ApimUserResponse> apimUserResponse,
            UserResponse putUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IAzureUserAuthenticationManagementService> azureUserAuthenticationManagementService,
            UserService userService)
        {
            userNote.AccountLockedDateTime = DateTime.Now.AddMinutes(-5);
            apimUserResponse.Body.Values.First().Properties.Note = JsonConvert.SerializeObject(userNote);
            apimUserResponse.Body.Values.First().Properties.State = "blocked";
            var expectedAuthenticatedValue = new GetUserAuthenticatedRequest(email, password);
            azureUserAuthenticationManagementService.Setup(x =>
                x.GetAuthentication<GetUserAuthenticatedResponse>(It.Is<GetUserAuthenticatedRequest>(c =>
                    c.AuthorizationHeaderValue.Equals(expectedAuthenticatedValue.AuthorizationHeaderValue)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(new GetUserAuthenticatedResponse(), HttpStatusCode.Unauthorized, "unauthorized"));
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json"))
                .ReturnsAsync(apimUserResponse);
            azureApimManagementService.Setup(x =>
                x.Put<UserResponse>(It.Is<CreateUserRequest>(c =>
                    c.PutUrl.Contains($"{apimUserResponse.Body.Values[0].Name}"))))
                .ReturnsAsync(new ApiResponse<UserResponse>(putUserResponse, HttpStatusCode.OK, ""));
            
            var actual = await userService.CheckUserAuthentication(email, password);

            actual.Should().BeEquivalentTo(apimUserResponse.Body.Values.First().Properties, options => options
                .Excluding(properties => properties.Note));
            actual.Note.Should().BeEquivalentTo(userNote);
            actual.Id.Should().Be(apimUserResponse.Body.Values.First().Name);
            actual.Authenticated.Should().BeFalse();
            azureApimManagementService.Verify(service => service.Put<UserResponse>(It.Is<CreateUserRequest>(request => 
                ((CreateUserRequestBody)request.Data).Properties.Note.Contains(@$"""FailedAuthCount"":3,""ThirdFailedAuthDateTime"":""{DateTime.Now:yyyy-MM-dd}T")
                && ((CreateUserRequestBody)request.Data).Properties.State.Equals("blocked")))
                , Times.Never);
        }
        
        [Test, MoqAutoData]
        public async Task And_Authenticating_After_10min_Lock_Period_Then_Resets_Count_And_Removes_Lock(
            string email,
            string password,
            UserNote userNote,
            ApiResponse<ApimUserResponse> firstApimUserResponse,
            ApiResponse<ApimUserResponse> secondApimUserResponse,
            UserResponse putUserResponse,
            GetUserAuthenticatedResponse apimAuthenticatedResponse,
            [Frozen] ApimDeveloperApiConfiguration config,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IAzureUserAuthenticationManagementService> azureUserAuthenticationManagementService,
            UserService userService)
        {
            userNote.AccountLockedDateTime = DateTime.Now.AddMinutes(-(config.AccountLockedDurationMinutes+1));
            firstApimUserResponse.Body.Values.First().Properties.Note = JsonConvert.SerializeObject(userNote);
            firstApimUserResponse.Body.Values.First().Properties.State = "blocked";
            secondApimUserResponse.Body.Values.First().Properties.Note = JsonConvert.SerializeObject(new UserNote());
            secondApimUserResponse.Body.Values.First().Properties.State = "active";
            var expectedAuthenticatedValue = new GetUserAuthenticatedRequest(email, password);
            azureUserAuthenticationManagementService.SetupSequence(x =>
                x.GetAuthentication<GetUserAuthenticatedResponse>(It.Is<GetUserAuthenticatedRequest>(c =>
                    c.AuthorizationHeaderValue.Equals(expectedAuthenticatedValue.AuthorizationHeaderValue)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(apimAuthenticatedResponse, HttpStatusCode.Forbidden, "forbidden"))
                .ReturnsAsync(new ApiResponse<GetUserAuthenticatedResponse>(apimAuthenticatedResponse, HttpStatusCode.OK, ""));
            azureApimManagementService.SetupSequence(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json"))
                .ReturnsAsync(firstApimUserResponse)
                .ReturnsAsync(secondApimUserResponse);
            azureApimManagementService.Setup(x =>
                x.Put<UserResponse>(It.Is<CreateUserRequest>(c =>
                    c.PutUrl.Contains($"{firstApimUserResponse.Body.Values[0].Name}"))))
                .ReturnsAsync(new ApiResponse<UserResponse>(putUserResponse, HttpStatusCode.OK, ""));
            
            var actual = await userService.CheckUserAuthentication(email, password);
            
            actual.Should().BeEquivalentTo(secondApimUserResponse.Body.Values.First().Properties, options => options
                .Excluding(properties => properties.Note)
                .Excluding(properties => properties.State));
            actual.Note.FailedAuthCount.Should().Be(0);
            actual.Note.AccountLockedDateTime.Should().NotHaveValue();
            actual.Id.Should().Be(secondApimUserResponse.Body.Values.First().Name);
            actual.Authenticated.Should().BeTrue();
            actual.State.Should().Be("active");
        }
    }
}