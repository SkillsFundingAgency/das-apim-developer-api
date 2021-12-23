using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Requests
{
    public class CreateUserRequest : IPutRequest
    {
        private readonly string _apimUserId;

        public CreateUserRequest(string apimUserId, UserDetails userDetails)
        {
            _apimUserId = apimUserId;
            Data = new CreateUserRequestBody
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
        }

        public string PutUrl => $"users/{_apimUserId}?api-version=2021-04-01-preview";
        public object Data { get; set; }
    }
}