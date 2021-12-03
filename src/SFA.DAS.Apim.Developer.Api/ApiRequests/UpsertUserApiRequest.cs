namespace SFA.DAS.Apim.Developer.Api.ApiRequests
{
    public class UpsertUserApiRequest
    {
        public string Email { get ; set ; }
        public string Password { get ; set ; }
        public string FirstName { get ; set ; }
        public string LastName { get ; set ; }
    }
}