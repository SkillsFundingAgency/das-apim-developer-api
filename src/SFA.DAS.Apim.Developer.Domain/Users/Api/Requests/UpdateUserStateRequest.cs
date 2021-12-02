using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Requests
{
    public class UpdateUserStateRequest : IPutRequest
    {
        private readonly string _apimUserId;

        public UpdateUserStateRequest (string apimUserId, UserDetails userDetails)
        {
            _apimUserId = apimUserId;
            Data = new CreateUserRequestBody
            {
                Properties = new ApimCreateUserProperties
                {
                    State = "active",
                    Email = userDetails.Email,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                }
            };
        }
        public string PutUrl => $"users/{_apimUserId}?api-version=2021-04-01-preview";
        public object Data { get; set; }
    }
}