using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Users.Api
{
    public class WhenBuildingGetUserAuthenticatedRequest
    {
        [Test, AutoData]
        public void Then_The_Url_And_Body_Is_Correctly_Constructed(string userName, string password)
        {
            var actual = new GetUserAuthenticatedRequest(userName, password);

            actual.GetUrl.Should().Be("identity?api-version=2023-09-01-preview");
            actual.AuthorizationHeaderScheme.Should().Be("Basic");
            actual.AuthorizationHeaderValue.Should().Be($"{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userName}:{password}"))}");
        }
    }
}