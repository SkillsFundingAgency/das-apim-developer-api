using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Data.UnitTests.DatabaseMock;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.ApimSubscriptionAuditRepository
{
    public class WhenDeletingASubscriptionAudit
    {
        private Mock<IApimDeveloperDataContext> _apimDeveloperDataContext;
        private Data.Repository.ApimSubscriptionAuditRepository _apimSubscriptionAuditRepository;
        private ApimSubscriptionAudit _apimSubscriptionAudit;

        [SetUp]
        public void Arrange()
        {
            _apimSubscriptionAudit = new ApimSubscriptionAudit
            {
                UserId = "ABC123",
                Action = "Deleted",
                Timestamp = DateTime.UtcNow,
                ProductName = "Api Key",
                ApimUserType = 3
            };

            _apimDeveloperDataContext = new Mock<IApimDeveloperDataContext>();
            _apimDeveloperDataContext.Setup(x => x.ApimSubscriptionAudit).ReturnsDbSet(new List<ApimSubscriptionAudit>());
            _apimSubscriptionAuditRepository = new Data.Repository.ApimSubscriptionAuditRepository(_apimDeveloperDataContext.Object);
        }

        [Test]
        public async Task Then_The_Subscription_Audit_Is_Deleted()
        {
            //Act
            await _apimSubscriptionAuditRepository.Insert(_apimSubscriptionAudit);

            //Assert
            _apimDeveloperDataContext.Verify(x => x.ApimSubscriptionAudit.AddAsync(_apimSubscriptionAudit, It.IsAny<CancellationToken>()), Times.Once);
            _apimDeveloperDataContext.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}