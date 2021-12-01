using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Requests
{
    public class UpdateUserStateRequest : IPutRequest
    {
        private readonly string _apimUserId;

        public UpdateUserStateRequest (string apimUserId)
        {
            _apimUserId = apimUserId;
            Data = new CreateUserRequestBody
            {
                Properties = new ApimCreateUserProperties
                {
                    State = "active"
                }
            };
        }
        public string PutUrl => $"users/{_apimUserId}?api-version=2021-04-01-preview";
        public object Data { get; set; }
    }
}