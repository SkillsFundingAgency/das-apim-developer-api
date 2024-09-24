using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<string>
    {
        public string Id { get ; set ; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string State { get ; set ; }
        public string ConfirmEmailLink { get ; set ; }
    }
}