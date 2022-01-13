using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserByEmail;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Queries
{
    public class WhenHandlingGetUserByEmailQuery
    {
        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Sent_To_The_User_Service(
            UserDetails user,
            GetUserByEmailQuery query,
            [Frozen] Mock<IUserService> mockUserService,
            GetUserByEmailQueryHandler handler)
        {
            mockUserService.Setup(service => service.GetUser(query.Email))
                .ReturnsAsync(user);

            var actual = await handler.Handle(query, CancellationToken.None);

            actual.User.Should().BeEquivalentTo(user);
        }
    }
}