using System;
using System.Collections.Generic;
using System.Threading;
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
    public class WhenAddingAApimUser
    {
        [Test, RecursiveMoqAutoData]
        public async Task ThenTheUserIsInserted(
            ApimUser user,
            [Frozen] Mock<IApimDeveloperDataContext> context,
            Data.Repository.ApimUserRepository repository)
        {
            //Arrange
            context.Setup(x => x.ApimUser).ReturnsDbSet(new List<ApimUser>()
            {
                Capacity = 0
            });
            
            //Act
            var actual = await repository.Insert(user);

            //Assert
            actual.Should().Be(user);
            context.Verify(x => x.ApimUser.AddAsync(It.IsAny<ApimUser>(), CancellationToken.None), Times.Once);
            context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}