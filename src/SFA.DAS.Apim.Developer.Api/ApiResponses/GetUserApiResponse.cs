using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserAuthenticated;

namespace SFA.DAS.Apim.Developer.Api.ApiResponses
{
    public class GetUserApiResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string State { get ; set ; }


        public static implicit operator GetUserApiResponse(GetUserAuthenticatedQueryResponse source)
        {
            return new GetUserApiResponse
            {
                Id = source.User.Id,
                Email = source.User.Email,
                FirstName = source.User.FirstName,
                LastName = source.User.LastName,
                State = source.User.State
            };
        }
    }
}