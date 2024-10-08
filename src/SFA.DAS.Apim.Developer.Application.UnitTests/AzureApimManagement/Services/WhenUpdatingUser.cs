using System;
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
using SFA.DAS.Apim.Developer.Domain.Users.Api.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenUpdatingUser
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_Updates_User_With_Values_Provided(
            UserDetails userDetails,
            UserResponse createUserApiResponse,
            ApimUserResponseItem apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            UserService userService)
        {
            apimUserResponse.Name = userDetails.Id;
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponseItem>(It.Is<GetApimUserByIdRequest>(c =>
                    c.GetUrl.Contains($"{userDetails.Id}")), "application/json"))
                .ReturnsAsync(new ApiResponse<ApimUserResponseItem>(apimUserResponse, HttpStatusCode.OK, ""));
            azureApimManagementService.Setup(x =>
                    x.Put<UserResponse>(It.Is<CreateUserRequest>(c => c.PutUrl.Contains($"users/{userDetails.Id}?") 
                                                                      && ((CreateUserRequestBody)c.Data).Properties.Email.Equals(userDetails.Email)
                                                                      && ((CreateUserRequestBody)c.Data).Properties.FirstName.Equals(userDetails.FirstName)
                                                                      && ((CreateUserRequestBody)c.Data).Properties.LastName.Equals(userDetails.LastName)
                                                                      && ((CreateUserRequestBody)c.Data).Properties.Note.Equals(JsonConvert.SerializeObject(userDetails.Note) )
                                                                      && ((CreateUserRequestBody)c.Data).Properties.State.Equals(userDetails.State)
                                                                      && ((CreateUserRequestBody)c.Data).Properties.Password.Equals(userDetails.Password)
                    )))
                .ReturnsAsync(new ApiResponse<UserResponse>(createUserApiResponse, HttpStatusCode.Created, ""));
            
            var actual = await userService.UpdateUser(userDetails);
            
            actual.Email.Should().Be(createUserApiResponse.Properties.Email);
            actual.FirstName.Should().Be(createUserApiResponse.Properties.FirstName);
            actual.LastName.Should().Be(createUserApiResponse.Properties.LastName);
            actual.Id.Should().Be(createUserApiResponse.Name);
            actual.State.Should().Be(createUserApiResponse.Properties.State);
            actual.Note.Should().BeNull();
            actual.Password.Should().BeNull();
        }
        
        [Test, RecursiveMoqAutoData]
        public async Task Then_If_The_User_Is_In_The_Portal_And_No_Email_Then_Updated_Through_Api_And_Non_Supplied_Values_Are_Not_Updated(
            Guid userId,
            UserResponse createUserApiResponse,
            UserNote userNote,
            ApimUserResponseItem apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            UserService userService)
        {
            var userDetails = new UserDetails
            {
                Id = userId.ToString(),
                Email = null,
                Note = null,
                Password = null,
                FirstName = null,
                LastName = null,
                State = "Active"
            };
            apimUserResponse.Name = userDetails.Id;
            apimUserResponse.Properties.Note = JsonConvert.SerializeObject(userNote);
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponseItem>(It.Is<GetApimUserByIdRequest>(c =>
                    c.GetUrl.Contains($"{userDetails.Id}")), "application/json"))
                .ReturnsAsync(new ApiResponse<ApimUserResponseItem>(apimUserResponse, HttpStatusCode.OK, ""));
            azureApimManagementService.Setup(x =>
                    x.Put<UserResponse>(It.Is<CreateUserRequest>(c => c.PutUrl.Contains($"users/{apimUserResponse.Name}?") 
                                                                      && ((CreateUserRequestBody)c.Data).Properties.Email.Equals(apimUserResponse.Properties.Email)
                                                                      && ((CreateUserRequestBody)c.Data).Properties.FirstName.Equals(apimUserResponse.Properties.FirstName)
                                                                      && ((CreateUserRequestBody)c.Data).Properties.LastName.Equals(apimUserResponse.Properties.LastName)
                                                                      && ((CreateUserRequestBody)c.Data).Properties.Note.Equals(apimUserResponse.Properties.Note)
                                                                      && ((CreateUserRequestBody)c.Data).Properties.State.Equals(userDetails.State)
                    )))
                .ReturnsAsync(new ApiResponse<UserResponse>(createUserApiResponse, HttpStatusCode.Created, ""));
            
            var actual = await userService.UpdateUser(userDetails);
            
            actual.Email.Should().Be(createUserApiResponse.Properties.Email);
            actual.FirstName.Should().Be(createUserApiResponse.Properties.FirstName);
            actual.LastName.Should().Be(createUserApiResponse.Properties.LastName);
            actual.Id.Should().Be(createUserApiResponse.Name);
            actual.State.Should().Be(createUserApiResponse.Properties.State);
        }

        [Test, MoqAutoData]
        public async Task Then_If_The_User_Does_Not_Exist_Then_Null_Is_Returned(
            UserDetails userDetails,
            UserResponse createUserApiResponse,
            ApimUserResponseItem apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponseItem>(It.Is<GetApimUserByIdRequest>(c =>
                    c.GetUrl.Contains($"{userDetails.Id}")), "application/json")).ReturnsAsync(new ApiResponse<ApimUserResponseItem>(null, HttpStatusCode.NotFound, ""));
            
            var actual = await userService.UpdateUser(userDetails);

            actual.Should().BeNull();
        }
    }
}