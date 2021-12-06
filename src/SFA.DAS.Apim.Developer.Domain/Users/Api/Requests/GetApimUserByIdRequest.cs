using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Users.Api.Requests
{
    public class GetApimUserByIdRequest : IGetRequest
    {
        private readonly string _id;

        public GetApimUserByIdRequest (string id)
        {
            _id = id;
        }

        public string GetUrl => $"users/{_id}?api-version=2021-04-01-preview";
    }
}