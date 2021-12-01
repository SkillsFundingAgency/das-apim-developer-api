using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingCreateUserRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Set(string userId, UserDetails userDetails)
        {
            var actual = new CreateUserRequest(userId, userDetails);

            actual.PutUrl.Should().Be($"users/{userId}?api-version=2021-04-01-preview");
        }

        [Test, AutoData]
        public void Then_The_Data_Is_Correctly_Set(string userId, UserDetails userDetails)
        {
            var expected = new CreateUserRequestBody
            {
                Properties = new ApimCreateUserProperties
                {
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Password = userDetails.Password,
                    State = "pending"
                }
            };
            
            var actual = new CreateUserRequest(userId, userDetails);
            
            ((CreateUserRequestBody)actual.Data).Should().BeEquivalentTo(expected);
        }
    }
}