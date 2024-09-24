using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserSubscriptions;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.ApiResponses
{
    public class WhenMappingFromMediatorResponseToGetUserSubscriptionsApiResponse
    {
        [Test, AutoData]
        public void Then_The_Values_Are_Mapped(GetUserSubscriptionsQueryResponse source)
        {
            var actual = (GetUserSubscriptionsApiResponse) source;
            
            actual.Subscriptions.Should().BeEquivalentTo(source.UserSubscriptions, options => options
                .Excluding(c=>c.Id)
                .Excluding(c=>c.PrimaryKey)
            );
            actual.Subscriptions.Select(c => c.Key).Should()
                .BeEquivalentTo(source.UserSubscriptions.Select(c => c.PrimaryKey).ToList());
        }
    }
}