using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Api.ApiResponses
{
    public class GetUserApiResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string State { get ; set ; }
        public bool Authenticated { get ; set ; }


        public static implicit operator GetUserApiResponse(UserDetails source)
        {
            return new GetUserApiResponse
            {
                Id = source.Id,
                Email = source.Email,
                FirstName = source.FirstName,
                LastName = source.LastName,
                State = source.State,
                Authenticated = source.Authenticated
            };
        }
    }
}