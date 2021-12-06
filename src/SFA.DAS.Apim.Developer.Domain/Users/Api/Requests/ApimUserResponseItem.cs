using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Requests
{
    public class ApimUserResponseItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }
    
    
    public class Properties
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("note")]
        public string Note { get ; set ; }
    }
}