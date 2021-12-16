using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserAuthenticated;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Queries
{
    public class WhenHandlingGetUserAuthenticatedQuery
    {
        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Sent_To_The_User_Service(
            UserDetails user,
            GetUserAuthenticatedQuery query,
            [Frozen] Mock<IUserService> userService,
            GetUserAuthenticatedQueryHandler handler)
        {
            userService.Setup(x => x.CheckUserAuthentication(query.Email, query.Password)).ReturnsAsync(user);

            var actual = await handler.Handle(query, CancellationToken.None);

            actual.User.Should().BeEquivalentTo(user);
        }
    }
}