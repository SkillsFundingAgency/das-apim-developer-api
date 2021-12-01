using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Users.Api
{
    public class WhenBuildingUpdateUserRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Set_And_State(string userId)
        {
            var expected = new CreateUserRequestBody
            {
                Properties = new ApimCreateUserProperties
                {
                    State = "active"
                }
            };

            var actual = new UpdateUserStateRequest(userId);

            actual.PutUrl.Should().Be($"users/{userId}?api-version=2021-04-01-preview");
            ((CreateUserRequestBody)actual.Data).Should().BeEquivalentTo(expected);
        }
    }
}