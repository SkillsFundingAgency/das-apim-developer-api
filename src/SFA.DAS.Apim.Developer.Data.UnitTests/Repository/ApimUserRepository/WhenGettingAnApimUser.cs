using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Data.UnitTests.DatabaseMock;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.ApimUserRepository
{
    public class WhenGettingAnApimUser
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_If_The_Record_Exists_It_Is_Returned_By_InternalId_And_Type(
            string internalUserId,
            int apimUserTypeId,
            ApimUser apimUserEntity,
            [Frozen] Mock<IApimDeveloperDataContext> mockDbContext,
            Data.Repository.ApimUserRepository repository)
        {

            var entities = new List<ApimUser>{
                apimUserEntity
            };
            //Arrange
            mockDbContext.Setup(x => x.ApimUser).ReturnsDbSet(entities);
            
            //Act
            var actual = await repository.GetByInternalIdAndType(apimUserEntity.InternalUserId, apimUserEntity.ApimUserTypeId);
            
            //Assert
            actual.Should().BeEquivalentTo(apimUserEntity);
        }
    }
}