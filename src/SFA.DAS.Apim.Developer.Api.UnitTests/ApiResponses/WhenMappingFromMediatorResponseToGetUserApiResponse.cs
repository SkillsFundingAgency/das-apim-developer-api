using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserAuthenticated;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.ApiResponses
{
    public class WhenMappingFromMediatorResponseToGetUserApiResponse
    {
        [Test, AutoData]
        public void Then_The_Fields_Are_Mapped(UserDetails source)
        {
            var actual = (GetUserApiResponse)source;
            
            actual.Should().BeEquivalentTo(source, options=> options
                .Excluding(c=>c.Password)
                .Excluding(c=>c.Note));
        }
    }
}