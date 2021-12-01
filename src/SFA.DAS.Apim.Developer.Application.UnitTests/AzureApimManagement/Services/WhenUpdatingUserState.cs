using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenUpdatingUserState
    {
        [Test, MoqAutoData]
        public async Task Then_If_The_User_Is_In_The_Portal_Then_Not_Created_Through_Api(
            string userId,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            UserService userService)
        {
            await userService.UpdateUserState(userId);
            
            azureApimManagementService.Verify(x =>
                x.Put<UserResponse>(It.Is<UpdateUserStateRequest>(c=>c.PutUrl.Contains(userId))), Times.Once);
        }
    }
}