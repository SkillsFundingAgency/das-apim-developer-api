using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Data.UnitTests.DatabaseMock;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.ApimUserRepository
{
    public class WhenAddingAApimUser
    {
        private Mock<IApimDeveloperDataContext> _apimDeveloperDataContext;
        private Data.Repository.ApimUserRepository _apimUserRepository;
        private ApimUser _apimUser;

        [SetUp]
        public void Arrange()
        {
            _apimUser = new ApimUser(){
                 Id = new Guid(),
                 ApimUserTypeId = 1,
                 InternalUserId = "123"
            };

            _apimDeveloperDataContext = new Mock<IApimDeveloperDataContext>();
            _apimDeveloperDataContext.Setup(x => x.ApimUser).ReturnsDbSet(new List<ApimUser>());
            _apimUserRepository = new Data.Repository.ApimUserRepository(_apimDeveloperDataContext.Object);
        }

        [Test]
        public async Task Then_The_ApimUser_Is_Added()
        {
            //Act
            await _apimUserRepository.Insert(_apimUser);
            
            //Assert
            _apimDeveloperDataContext.Verify(x=>x.ApimUser.AddAsync(_apimUser, It.IsAny<CancellationToken>()), Times.Once);
            _apimDeveloperDataContext.Verify(x=>x.SaveChanges(), Times.Once);
        }
    }
}