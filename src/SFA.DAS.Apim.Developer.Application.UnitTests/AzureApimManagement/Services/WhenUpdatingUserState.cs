using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenUpdatingUserState
    {
        [Test, MoqAutoData]
        public async Task Then_The_User_Is_Looked_Up_And_Updated(
            string email,
            ApimUserResponse apimUserResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            UserService userService)
        {
            apimUserResponse.Count = 1;
            apimUserResponse.Values.First().Properties.Email = email;
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json")).ReturnsAsync(new ApiResponse<ApimUserResponse>
            (
                apimUserResponse, HttpStatusCode.OK, ""
            ));
            
            await userService.UpdateUserState(email);

            var userId = apimUserResponse.Values.First().Name;
            var firstName = apimUserResponse.Values.First().Properties.FirstName;
            var lastName = apimUserResponse.Values.First().Properties.LastName;
            
            azureApimManagementService.Verify(x =>
                x.Put<UserResponse>(It.Is<UpdateUserStateRequest>(c=>c.PutUrl.Contains(userId)
                && ((CreateUserRequestBody)c.Data).Properties.Email.Equals(email)
                && ((CreateUserRequestBody)c.Data).Properties.FirstName.Equals(firstName)
                && ((CreateUserRequestBody)c.Data).Properties.LastName.Equals(lastName)
                && ((CreateUserRequestBody)c.Data).Properties.State.Equals("active")
                )), Times.Once);
        }

        [Test, MoqAutoData]
        public void Then_If_User_Not_Found_Then_Validation_Exception_Thrown(
            string email,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            UserService userService)
        {
            azureApimManagementService.Setup(x =>
                x.Get<ApimUserResponse>(It.Is<GetApimUserRequest>(c =>
                    c.GetUrl.Contains($"'{email}'")), "application/json")).ReturnsAsync(new ApiResponse<ApimUserResponse>
            (
                new ApimUserResponse
                {
                    Count = 0
                }, HttpStatusCode.OK, ""
            ));

            Assert.ThrowsAsync<ValidationException>(() => userService.UpdateUserState(email));
        }
    }
}