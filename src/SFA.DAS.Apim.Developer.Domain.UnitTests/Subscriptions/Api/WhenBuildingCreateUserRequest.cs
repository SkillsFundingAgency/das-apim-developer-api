using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingCreateUserRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Set(string userId)
        {
            var actual = new CreateUserRequest(userId);

            actual.PutUrl.Should().Be($"users/{userId}?api-version=2021-04-01-preview");
        }

        [Test, AutoData]
        public void Then_The_Data_Is_Correctly_Set(string userId)
        {
            var expected = new CreateUserRequestBody
            {
                Properties = new ApimCreateUserProperties
                {
                    Email = "test@testing.com",
                    FirstName = "firstname",
                    LastName = "lastname"
                }
            };
            
            var actual = new CreateUserRequest(userId);
            
            ((CreateUserRequestBody)actual.Data).Should().BeEquivalentTo(expected);
        }
    }
}