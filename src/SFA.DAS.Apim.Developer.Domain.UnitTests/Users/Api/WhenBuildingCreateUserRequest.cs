using System.Collections.Generic;
using System.Web;
using AutoFixture.NUnit3;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Users.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Users.Api
{
    public class WhenBuildingCreateUserRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Set(string userId, UserDetails userDetails)
        {
            userId += "+more things";
            var encodedUserId = HttpUtility.UrlEncode(userId);
            
            var actual = new CreateUserRequest(userId, userDetails);

            actual.PutUrl.Should().Be($"users/{encodedUserId}?api-version=2021-04-01-preview");
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
                    State = userDetails.State,
                    Note = JsonConvert.SerializeObject(userDetails.Note),
                    Identities = new List<Identities>
                    {
                        new Identities
                        {
                            Id = userDetails.Email,
                            Provider = "Basic"
                        }
                    }
                }
            };
            
            var actual = new CreateUserRequest(userId, userDetails);
            
            ((CreateUserRequestBody)actual.Data).Should().BeEquivalentTo(expected);
        }
        
        [Test, AutoData]
        public void And_Note_Null_Then_Not_Set(string userId, UserDetails userDetails)
        {
            userDetails.Note = null;
            var expected = new CreateUserRequestBody
            {
                Properties = new ApimCreateUserProperties
                {
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Password = userDetails.Password,
                    State = userDetails.State,
                    Note = null,
                    Identities = new List<Identities>
                    {
                        new Identities
                        {
                            Id = userDetails.Email,
                            Provider = "Basic"
                        }
                    }
                }
            };
            
            var actual = new CreateUserRequest(userId, userDetails);
            
            ((CreateUserRequestBody)actual.Data).Should().BeEquivalentTo(expected);
        }
    }
}