using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.UpdateUserState
{
    public class UpdateUserStateCommand : IRequest<Unit>
    {
        public string UserEmail { get; set; }
    }
}