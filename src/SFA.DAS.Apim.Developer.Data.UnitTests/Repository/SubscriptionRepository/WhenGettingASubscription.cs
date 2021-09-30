using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.SubscriptionRepository
{
    public class WhenGettingASubscription
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_If_The_Record_Exists_It_Is_Returned_By_Id(
            Guid id,
            Subscription subscriptionEntity,
            [Frozen] Mock<IApimDeveloperDataContext> mockDbContext,
            Data.Repository.SubscriptionRepository repository)
        {
            //Arrange
            mockDbContext.Setup(x => x.Subscription.FindAsync(id))
                .ReturnsAsync(subscriptionEntity);
            
            //Act
            var actual = await repository.Get(id);
            
            //Assert
            actual.Should().BeEquivalentTo(subscriptionEntity);
        }
    }
}