using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Requests
{
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