using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Data.UnitTests.DatabaseMock;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.SubscriptionAuditRepository
{
    public class WhenAddingASubscription
    {
        private Mock<IApimDeveloperDataContext> _apimDeveloperDataContext;
        private Data.Repository.SubscriptionAuditRepository _subscriptionAuditRepository;
        private SubscriptionAudit _subscriptionAudit;

        [SetUp]
        public void Arrange()
        {
            _subscriptionAudit = new SubscriptionAudit()
            {
                SubscriptionId = new Guid(),
                UserRef = "123",
                Action = "Created",
                Timestamp = DateTime.UtcNow
            };

            _apimDeveloperDataContext = new Mock<IApimDeveloperDataContext>();
            _apimDeveloperDataContext.Setup(x => x.SubscriptionAudit).ReturnsDbSet(new List<SubscriptionAudit>());
            _subscriptionAuditRepository = new Data.Repository.SubscriptionAuditRepository(_apimDeveloperDataContext.Object);
        }

        [Test]
        public async Task Then_The_Subscription_Is_Added()
        {
            //Act
            await _subscriptionAuditRepository.Insert(_subscriptionAudit);

            //Assert
            _apimDeveloperDataContext.Verify(x => x.SubscriptionAudit.AddAsync(_subscriptionAudit, It.IsAny<CancellationToken>()), Times.Once);
            _apimDeveloperDataContext.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}