using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Data.UnitTests.DatabaseMock;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.ApimAuditRepository
{
    public class WhenAddingAnApimAudit
    {
        private Mock<IApimDeveloperDataContext> _apimDeveloperDataContext;
        private Data.Repository.ApimAuditRepository _apimAuditRepository;
        private ApimAudit _apimAudit;

        [SetUp]
        public void Arrange()
        {
            _apimAudit = new ApimAudit()
            {
                ApimUserId = new Guid(),
                Action = "Created",
                Timestamp = DateTime.UtcNow
            };

            _apimDeveloperDataContext = new Mock<IApimDeveloperDataContext>();
            _apimDeveloperDataContext.Setup(x => x.ApimAudit).ReturnsDbSet(new List<ApimAudit>());
            _apimAuditRepository = new Data.Repository.ApimAuditRepository(_apimDeveloperDataContext.Object);
        }

        [Test]
        public async Task Then_The_Audit_Is_Added()
        {
            //Act
            await _apimAuditRepository.Insert(_apimAudit);

            //Assert
            _apimDeveloperDataContext.Verify(x => x.ApimAudit.AddAsync(_apimAudit, It.IsAny<CancellationToken>()), Times.Once);
            _apimDeveloperDataContext.Verify(x => x.SaveChanges(), Times.Once);
        }
    }
}