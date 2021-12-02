using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserAuthenticated;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.ApiResponses
{
    public class WhenMappingFromMediatorResponseToGetUserApiResponse
    {
        [Test, AutoData]
        public void Then_The_Fields_Are_Mapped(GetUserAuthenticatedQueryResponse source)
        {
            var actual = (GetUserApiResponse)source;
            
            actual.Should().BeEquivalentTo(source.User, options=> options.Excluding(c=>c.Password));
        }
        
    }
}