using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription
{
    public class CreateUserSubscriptionCommandHandler : IRequestHandler<CreateUserSubscriptionCommand, CreateUserSubscriptionCommandResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IValidator<CreateUserSubscriptionCommand> _validator;

        public CreateUserSubscriptionCommandHandler(ISubscriptionService subscriptionService, IValidator<CreateUserSubscriptionCommand> validator)
        {
            _subscriptionService = subscriptionService;
            _validator = validator;
        }
        public async Task<CreateUserSubscriptionCommandResponse> Handle(CreateUserSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ValidationException(validationResult.DataAnnotationResult,null, null);
            }
            
            var subscriptionId = await _subscriptionService.CreateUserSubscription(request.InternalUserId, request.ApimUserType, request.ProductName, request.ApimUserId);

            return new CreateUserSubscriptionCommandResponse()
            {
                SubscriptionId = subscriptionId
            }; //TODO: return something more useful?
        }
    }
}