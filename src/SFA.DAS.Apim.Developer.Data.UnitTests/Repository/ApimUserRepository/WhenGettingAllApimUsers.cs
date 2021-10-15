using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Data.UnitTests.DatabaseMock;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.ApimUserRepository
{
    public class WhenGettingAllApimUsers
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_All_ApimUsers_Are_Returned(
            ICollection<ApimUser> apimUserEntities,
            [Frozen] Mock<IApimDeveloperDataContext> mockDbContext,
            Data.Repository.ApimUserRepository repository)
        {
            //Arrange
            mockDbContext.Setup(x => x.ApimUser).ReturnsDbSet(apimUserEntities);
            
            //Act
            var actual = await repository.GetAll();
            
            //Assert
            actual.Should().BeEquivalentTo(apimUserEntities);
        }
    }
}