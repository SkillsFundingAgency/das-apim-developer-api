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

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.SubscriptionRepository
{
    public class WhenGettingAllSubscriptions
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_All_Subscriptions_Are_Returned(
            ICollection<Subscription> subscriptionEntities,
            [Frozen] Mock<IApimDeveloperDataContext> mockDbContext,
            Data.Repository.SubscriptionRepository repository)
        {
            //Arrange
            mockDbContext.Setup(x => x.Subscription).ReturnsDbSet(subscriptionEntities);
            
            //Act
            var actual = await repository.GetAll();
            
            //Assert
            actual.Should().BeEquivalentTo(subscriptionEntities);
        }
    }
}