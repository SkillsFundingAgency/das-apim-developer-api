using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenGettingUser
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_The_Repository_Is_Called_And_User_Returned(
            string internalUserId,
            ApimUserType apimUserType,
            ApimUser user,
            [Frozen] Mock<IApimUserRepository> apimUserRepository, 
            UserService userService)
        {
            apimUserRepository.Setup(x => x.GetByInternalIdAndType(internalUserId, (int)apimUserType))
                .ReturnsAsync(user);
            
            var actual = await userService.GetUser(internalUserId, apimUserType);
            
            actual.Should().BeEquivalentTo(user);
        }
    }
}