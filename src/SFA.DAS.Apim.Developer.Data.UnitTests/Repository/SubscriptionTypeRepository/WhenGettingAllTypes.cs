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

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.SubscriberTypeRepository
{
    public class WhenGettingAllTypes
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_All_Subscriber_Types_Are_Returned(
            ICollection<SubscriberType> subscriptionTypes,
            [Frozen] Mock<IApimDeveloperDataContext> mockDbContext,
            Data.Repository.SubscriberTypeRepository repository)
        {
            //Arrange
            mockDbContext.Setup(x => x.SubscriberType).ReturnsDbSet(subscriptionTypes);
            
            //Act
            var actual = await repository.GetAll();
            
            //Assert
            actual.Should().BeEquivalentTo(subscriptionTypes);
        }
    }
}