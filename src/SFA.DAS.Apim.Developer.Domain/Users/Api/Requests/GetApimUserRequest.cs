using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Requests
{
    public class GetApimUserRequest : IGetRequest
    {
        private readonly string _email;

        public GetApimUserRequest(string email)
        {
            _email = email;
        }

        public string GetUrl => $"users?$filter=email eq '{_email}'&api-version=2021-04-01-preview";
    }
    
    public class ApimUserResponse
    {
        [JsonProperty("value")]
        public List<ApimUserResponseValue> Values { get; set; }
        
        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public class ApimUserResponseValue
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
    }
}