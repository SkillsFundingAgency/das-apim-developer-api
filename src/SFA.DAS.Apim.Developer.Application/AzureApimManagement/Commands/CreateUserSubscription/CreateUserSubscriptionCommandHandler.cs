using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription
{
    public class CreateUserSubscriptionCommandHandler : IRequestHandler<CreateUserSubscriptionCommand, CreateUserSubscriptionCommandResponse>
    {
        public Task<CreateUserSubscriptionCommandResponse> Handle(CreateUserSubscriptionCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}