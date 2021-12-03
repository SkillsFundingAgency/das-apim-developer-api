using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Responses
{
    public class UserResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "properties")]
        public ApimUserContract Properties { get; set; }

        public class ApimUserContract
        {
            [JsonProperty(PropertyName = "email")]
            public string Email { get; set; }
            [JsonProperty(PropertyName = "firstName")]
            public string FirstName { get; set; }
            [JsonProperty(PropertyName = "lastName")]
            public string LastName { get; set; }
            [JsonProperty(PropertyName = "state")]
            public string State { get ; set ; }
        }
    }
}