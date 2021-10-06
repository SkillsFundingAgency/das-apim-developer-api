using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Data.UnitTests.DatabaseMock;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.ApimUserTypeRepository
{
    public class WhenGettingAnItem
    {
        private Mock<IApimDeveloperDataContext> _apimDeveloperDataContext;
        private Data.Repository.ApimUserTypeRepository _apimUserTypeRepository;
        private List<ApimUserType> _apimUserTypes;

        private const string ExpectedApimUserTypeName = "Employer";


        [SetUp]
        public void Arrange()
        {
            _apimUserTypes = new List<ApimUserType>(){
                new ApimUserType() {
                    Id = 1,
                    Name = ExpectedApimUserTypeName
                },
                new ApimUserType() {
                    Id = 2,
                    Name = "Provider"
                }
            };

            _apimDeveloperDataContext = new Mock<IApimDeveloperDataContext>();
            _apimDeveloperDataContext.Setup(x => x.ApimUserType).ReturnsDbSet(_apimUserTypes);
            _apimUserTypeRepository = new Data.Repository.ApimUserTypeRepository(_apimDeveloperDataContext.Object);
        }

        [Test]
        public async Task Then_Get_The_Subscriber_Type()
        {
            //Act
            var subscriberType = await _apimUserTypeRepository.Get(ExpectedApimUserTypeName);

            //Assert
            Assert.IsNotNull(subscriberType);
            subscriberType.Should().BeEquivalentTo(_apimUserTypes.SingleOrDefault(c => c.Name.Equals(ExpectedApimUserTypeName)));
        }
    }
}