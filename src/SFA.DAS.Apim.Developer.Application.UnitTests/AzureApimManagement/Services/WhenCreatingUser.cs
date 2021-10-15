using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenCreatingUser
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_If_The_User_Does_Not_Exist_It_Is_Created_In_The_Database_And_On_Service(
            string internalUserId, 
            ApimUserType apimUserType, 
            string productName, 
            Guid apimUserId,
            ApimUser apimUser,
            UserDetails userDetails,
            ApiResponse<CreateUserResponse> createUserApiResponse,
            ApiResponse<CreateSubscriptionResponse> createSubscriptionApiResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            [Frozen] Mock<IApimUserRepository> apimUserRepository, 
            UserService userService)
        {
            createUserApiResponse.Body.Name = apimUserId.ToString();
            apimUserRepository.Setup(x => x.GetByInternalIdAndType(internalUserId, (int) apimUserType)).ReturnsAsync((ApimUser)null);
            azureApimManagementService.Setup(x =>
                    x.Put<CreateUserResponse>(It.Is<CreateUserRequest>(c => c.PutUrl.Contains($"users/") 
                        && ((CreateUserRequestBody)c.Data).Properties.Email.Equals(userDetails.EmailAddress)
                        && ((CreateUserRequestBody)c.Data).Properties.FirstName.Equals(userDetails.FirstName)
                        && ((CreateUserRequestBody)c.Data).Properties.LastName.Equals(userDetails.LastName)
                    )))
                .ReturnsAsync(createUserApiResponse);
            apimUserRepository.Setup(x=>x.Insert(It.Is<ApimUser>(c=>
                c.ApimUserTypeId.Equals((int) apimUserType)
                && c.InternalUserId.Equals(internalUserId)
            ))).ReturnsAsync(apimUser);
            

            var actual = await userService.CreateUser(internalUserId, userDetails, apimUserType);
            
            actual.Should().Be(createUserApiResponse.Body.Name);
        }
        
        [Test, RecursiveMoqAutoData]
        public async Task Then_If_The_User_Is_In_The_Portal_But_Not_Database_Then_Not_Created_Through_Api(
            string internalUserId, 
            ApimUserType apimUserType, 
            Guid apimUserId,
            ApimUser apimUser,
            UserDetails userDetails,
            CreateUserResponse createUserResponse,
            ApiResponse<CreateSubscriptionResponse> createSubscriptionApiResponse,
            ApiResponse<ApimUserResponse> apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            [Frozen] Mock<IApimUserRepository> apimUserRepository,  
            UserService userService)
        {
            createUserResponse.Name = apimUserId.ToString();
            var createUserApiResponse =
                new ApiResponse<CreateUserResponse>(createUserResponse, HttpStatusCode.BadRequest, "");
            apimUserRepository.Setup(x => x.GetByInternalIdAndType(internalUserId, (int) apimUserType)).ReturnsAsync((ApimUser)null);
            azureApimManagementService.Setup(x =>
                    x.Put<CreateUserResponse>(It.Is<CreateUserRequest>(c => c.PutUrl.Contains($"users/"))))
                .ReturnsAsync(createUserApiResponse);
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{userDetails.EmailAddress}'")))).ReturnsAsync(apimUserResponse);
            
            apimUserRepository.Setup(x=>x.Insert(It.Is<ApimUser>(c=>
                c.ApimUserTypeId.Equals((int) apimUserType)
                && c.InternalUserId!=Guid.Empty.ToString()
            ))).ReturnsAsync(apimUser);
            
        
            var actual = await userService.CreateUser(internalUserId, userDetails, apimUserType);
            
            actual.Should().Be(apimUserResponse.Body.Properties.First().Name);
        }
    }
}