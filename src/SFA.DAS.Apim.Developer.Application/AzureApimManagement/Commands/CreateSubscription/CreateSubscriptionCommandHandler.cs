using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Extensions;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateSubscription
{
    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, CreateSubscriptionCommandResponse>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IValidator<CreateSubscriptionCommand> _validator;

        public CreateSubscriptionCommandHandler(ISubscriptionService subscriptionService, IValidator<CreateSubscriptionCommand> validator)
        {
            _subscriptionService = subscriptionService;
            _validator = validator;
        }
        public async Task<CreateSubscriptionCommandResponse> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ValidationException(validationResult.DataAnnotationResult, null, null);
            }

            var subscriptionResponse = await _subscriptionService.CreateSubscription(
                request.InternalUserId,
                request.InternalUserId.ApimUserType(),
                request.ProductName);

            return new CreateSubscriptionCommandResponse
            {
                SubscriptionId = subscriptionResponse.Name,
                PrimaryKey = subscriptionResponse.PrimaryKey
            };
        }
    }
}