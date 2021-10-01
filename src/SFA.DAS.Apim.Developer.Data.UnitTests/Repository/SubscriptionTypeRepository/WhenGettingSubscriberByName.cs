using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Data.UnitTests.DatabaseMock;
using SFA.DAS.Apim.Developer.Domain.Entities;

namespace SFA.DAS.Apim.Developer.Data.UnitTests.Repository.SubscriptionTypeRepository
{
    public class WhenGettingAnItem
    {
        private Mock<IApimDeveloperDataContext> _apimDeveloperDataContext;
        private Data.Repository.SubscriberTypeRepository _subscriberTypeRepository;
        private List<SubscriberType> _subscriberTypes;

        private const string ExpectedSubscriberTypeName = "Employer";


        [SetUp]
        public void Arrange()
        {
            _subscriberTypes = new List<SubscriberType>(){
                new SubscriberType() {
                    Id = 1,
                    Name = ExpectedSubscriberTypeName
                },
                new SubscriberType() {
                    Id = 2,
                    Name = "Provider"
                }
            };

            _apimDeveloperDataContext = new Mock<IApimDeveloperDataContext>();
            _apimDeveloperDataContext.Setup(x => x.SubscriberType).ReturnsDbSet(_subscriberTypes);
            _subscriberTypeRepository = new Data.Repository.SubscriberTypeRepository(_apimDeveloperDataContext.Object);
        }

        [Test]
        public async Task Then_Get_The_Subscriber_Type()
        {
            //Act
            var subscriberType = await _subscriberTypeRepository.Get(ExpectedSubscriberTypeName);

            //Assert
            Assert.IsNotNull(subscriberType);
            subscriberType.Should().BeEquivalentTo(_subscriberTypes.SingleOrDefault(c => c.Name.Equals(ExpectedSubscriberTypeName)));
        }
    }
}