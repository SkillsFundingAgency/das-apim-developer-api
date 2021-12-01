using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests
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
                    State = "pending"
                }
            };
        }

        public string PutUrl => $"users/{_apimUserId}?api-version=2021-04-01-preview";
        public object Data { get; set; }
    }

    public class CreateUserRequestBody
    {
        [JsonProperty(PropertyName = "properties")]
        public ApimCreateUserProperties Properties { get; set; }
    }

    public class ApimCreateUserProperties
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get ; set ; }
        [JsonProperty(PropertyName = "password")]
        public string Password { get ; set ; }
    }
}