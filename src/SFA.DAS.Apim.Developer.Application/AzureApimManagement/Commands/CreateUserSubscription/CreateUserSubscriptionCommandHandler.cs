using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription
{
    public class CreateUserSubscriptionCommandHandler : IRequestHandler<CreateUserSubscriptionCommand, CreateUserSubscriptionCommandResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        public CreateUserSubscriptionCommandHandler(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        public async Task<CreateUserSubscriptionCommandResponse> Handle(CreateUserSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var subscriptionId = await _subscriptionService.CreateUserSubscription(request.InternalUserId, request.ApimUserType, request.ProductName);

            return new CreateUserSubscriptionCommandResponse()
            {
                SubscriptionId = subscriptionId
            }; //TODO: return something more useful?
        }
    }
}