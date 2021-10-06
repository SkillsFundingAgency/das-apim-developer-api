using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api
{
    public class CreateUserRequest : IPutRequest
    {
        private readonly string _apimUserId;

        public CreateUserRequest(string apimUserId)
        {
            _apimUserId = apimUserId;
            Data = new CreateUserRequestBody
            {
                Properties = new ApimCreateUserProperties
                {
                    //TODO: what to set these to
                    Email = "test@testing.com",
                    FirstName = "firstname",
                    LastName = "lastname"
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
    }
}