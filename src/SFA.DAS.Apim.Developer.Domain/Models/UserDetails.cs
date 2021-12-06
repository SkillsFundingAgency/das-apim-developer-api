namespace SFA.DAS.Apim.Developer.Domain.Models
{
    public class UserDetails
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get ; set ; }
        public string State { get; set; }
        public string Note { get ; set ; }
        public bool Authenticated { get; set; }
    }
}