using Newtonsoft.Json;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Responses
{
    public class GetUserAuthenticatedResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}