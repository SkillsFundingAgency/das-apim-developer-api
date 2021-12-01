using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses
{
    public class CreateUserResponse
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
        }
    }
}