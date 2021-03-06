using System.Collections.Generic;
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
        [JsonProperty(PropertyName = "identities")]
        public List<Identities> Identities { get ; set ; }
        [JsonProperty(PropertyName = "note")]
        public string Note { get ; set ; }
    }

    public class Identities
    {
        public string Provider { get; set; }
        public string Id { get; set; }
    }
}