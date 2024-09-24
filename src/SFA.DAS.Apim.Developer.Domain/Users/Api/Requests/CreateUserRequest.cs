using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Requests
{
    public class CreateUserRequest : IPutRequest
    {
        private readonly string _apimUserId;

        public CreateUserRequest(string apimUserId, UserDetails userDetails)
        {
            _apimUserId = HttpUtility.UrlEncode(apimUserId);
            Data = new CreateUserRequestBody
            {
                Properties = new ApimCreateUserProperties
                {
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Password = userDetails.Password,
                    State = userDetails.State,
                    Note = userDetails.Note == null ? null : JsonConvert.SerializeObject(userDetails.Note),
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

        public string PutUrl => $"users/{_apimUserId}?api-version=2023-09-01-preview";
        public object Data { get; set; }
    }
}