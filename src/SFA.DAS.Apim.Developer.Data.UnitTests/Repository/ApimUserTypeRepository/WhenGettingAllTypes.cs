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

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.ApimUserTypeRepository
{
    public class WhenGettingAllTypes
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_All_ApimUser_Types_Are_Returned(
            ICollection<ApimUserType> apimUserTypes,
            [Frozen] Mock<IApimDeveloperDataContext> mockDbContext,
            Data.Repository.ApimUserTypeRepository repository)
        {
            //Arrange
            mockDbContext.Setup(x => x.ApimUserType).ReturnsDbSet(apimUserTypes);
            
            //Act
            var actual = await repository.GetAll();
            
            //Assert
            actual.Should().BeEquivalentTo(apimUserTypes);
        }
    }
}