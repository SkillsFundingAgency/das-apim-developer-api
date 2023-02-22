using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Extensions;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.DeleteSubscription
{
    public class DeleteSubscriptionCommandHandler : IRequestHandler<DeleteSubscriptionCommand, Unit>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IValidator<DeleteSubscriptionCommand> _validator;

        public DeleteSubscriptionCommandHandler(ISubscriptionService subscriptionService, IValidator<DeleteSubscriptionCommand> validator)
        {
            _subscriptionService = subscriptionService;
            _validator = validator;
        }
        public async Task<Unit> Handle(DeleteSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ValidationException(validationResult.DataAnnotationResult, null, null);
            }

            await _subscriptionService.DeleteSubscription(
                request.InternalUserId,
                request.InternalUserId.ApimUserType(),
                request.ProductName);

            return Unit.Value;
        }
    }
}