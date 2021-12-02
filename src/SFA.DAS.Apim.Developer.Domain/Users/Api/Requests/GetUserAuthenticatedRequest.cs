using System;
using Newtonsoft.Json;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Requests
{
    public class GetUserAuthenticatedRequest : IGetUserAuthenticationRequest
    {

        public GetUserAuthenticatedRequest(string userName, string password)
        {
            AuthorizationHeaderScheme = "Basic";
            AuthorizationHeaderValue = $"{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userName}:{password}"))}";
        }

        public string AuthorizationHeaderScheme { get ;}

        public string GetUrl => "identity?api-version=2021-04-01-preview";
        public string AuthorizationHeaderValue { get; }
    }

    public class GetUserAuthenticatedResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}