using System;
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
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenCreatingUser
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_If_The_User_Does_Not_Exist_It_Is_Created_On_The_Service_And_Marked_As_Pending(
            UserDetails userDetails,
            UserResponse createUserApiResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{userDetails.Email}'")), "application/json")).ReturnsAsync(
                new ApiResponse<ApimUserResponse>(
                    new ApimUserResponse
                    {
                        Count = 0
                    }, HttpStatusCode.NotFound, ""));
            
            azureApimManagementService.Setup(x =>
                    x.Put<UserResponse>(It.Is<CreateUserRequest>(c => c.PutUrl.Contains($"users/{userDetails.Id}?") 
                        && ((CreateUserRequestBody)c.Data).Properties.Email.Equals(userDetails.Email)
                        && ((CreateUserRequestBody)c.Data).Properties.FirstName.Equals(userDetails.FirstName)
                        && ((CreateUserRequestBody)c.Data).Properties.LastName.Equals(userDetails.LastName)
                        && ((CreateUserRequestBody)c.Data).Properties.Password.Equals(userDetails.Password)
                        && ((CreateUserRequestBody)c.Data).Properties.State.Equals("pending")
                    )))
                .ReturnsAsync(new ApiResponse<UserResponse>(createUserApiResponse, HttpStatusCode.Created, ""));
            
            var actual = await userService.UpsertUser(userDetails);
            
            actual.Email.Should().Be(createUserApiResponse.Properties.Email);
            actual.FirstName.Should().Be(createUserApiResponse.Properties.FirstName);
            actual.LastName.Should().Be(createUserApiResponse.Properties.LastName);
            actual.Id.Should().Be(createUserApiResponse.Name);
        }
        
        [Test, RecursiveMoqAutoData]
        public async Task Then_If_The_User_Is_In_The_Portal_Then_Updated_Through_Api(
            UserDetails userDetails,
            UserResponse createUserApiResponse,
            ApiResponse<CreateSubscriptionResponse> createSubscriptionApiResponse,
            ApiResponse<ApimUserResponse> apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{userDetails.Email}'")), "application/json")).ReturnsAsync(apimUserResponse);
            azureApimManagementService.Setup(x =>
                    x.Put<UserResponse>(It.Is<CreateUserRequest>(c => c.PutUrl.Contains($"users/{apimUserResponse.Body.Values.First().Name}?") 
                              && ((CreateUserRequestBody)c.Data).Properties.Email.Equals(userDetails.Email)
                              && ((CreateUserRequestBody)c.Data).Properties.FirstName.Equals(userDetails.FirstName)
                              && ((CreateUserRequestBody)c.Data).Properties.LastName.Equals(userDetails.LastName)
                              && ((CreateUserRequestBody)c.Data).Properties.Password.Equals(userDetails.Password)
                              && ((CreateUserRequestBody)c.Data).Properties.State.Equals(userDetails.State)
                    )))
                .ReturnsAsync(new ApiResponse<UserResponse>(createUserApiResponse, HttpStatusCode.Created, ""));
            
            
            var actual = await userService.UpsertUser(userDetails);
            
            actual.Email.Should().Be(createUserApiResponse.Properties.Email);
            actual.FirstName.Should().Be(createUserApiResponse.Properties.FirstName);
            actual.LastName.Should().Be(createUserApiResponse.Properties.LastName);
            actual.Id.Should().Be(createUserApiResponse.Name);
            actual.State.Should().Be(createUserApiResponse.Properties.State);
        }

        [Test, RecursiveMoqAutoData]
        public void Then_If_Error_From_Api_Then_Exception_Thrown(
            UserDetails userDetails,
            ApiResponse<CreateSubscriptionResponse> createSubscriptionApiResponse,
            ApiResponse<ApimUserResponse> apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{userDetails.Email}'")), "application/json")).ReturnsAsync(
                new ApiResponse<ApimUserResponse>(
                    new ApimUserResponse
                    {
                        Count = 0
                    }, HttpStatusCode.NotFound, ""));
            azureApimManagementService.Setup(x =>
                x.Put<UserResponse>(It.IsAny<CreateUserRequest>())).ReturnsAsync(new ApiResponse<UserResponse>(null, HttpStatusCode.BadRequest, "Error"));

            Assert.ThrowsAsync<Exception>(() => userService.UpsertUser(userDetails));
        }
    }
}