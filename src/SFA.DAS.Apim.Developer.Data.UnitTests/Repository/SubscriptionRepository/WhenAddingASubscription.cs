using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Data.UnitTests.DatabaseMock;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.SubscriptionRepository
{
    public class WhenAddingASubscription
    {
        private Mock<IApimDeveloperDataContext> _apimDeveloperDataContext;
        private Data.Repository.SubscriptionRepository _subscriptionRepository;
        private Subscription _subscription;

        [SetUp]
        public void Arrange()
        {
            _subscription = new Subscription(){
                ExternalSubscriberId = "123",
                ExternalSubscriptionId = 1,
                SubscriberTypeId = 1
            };

            _apimDeveloperDataContext = new Mock<IApimDeveloperDataContext>();
            _apimDeveloperDataContext.Setup(x => x.Subscription).ReturnsDbSet(new List<Subscription>());
            _subscriptionRepository = new Data.Repository.SubscriptionRepository(_apimDeveloperDataContext.Object);
        }

        [Test]
        public async Task Then_The_Subscription_Is_Added()
        {
            //Act
            await _subscriptionRepository.Insert(_subscription);
            
            //Assert
            _apimDeveloperDataContext.Verify(x=>x.Subscription.AddAsync(_subscription, It.IsAny<CancellationToken>()), Times.Once);
            _apimDeveloperDataContext.Verify(x=>x.SaveChanges(), Times.Once);
        }
    }
}