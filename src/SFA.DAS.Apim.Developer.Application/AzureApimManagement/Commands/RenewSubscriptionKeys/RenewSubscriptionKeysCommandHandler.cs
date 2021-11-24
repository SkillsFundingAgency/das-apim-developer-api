using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.RenewSubscriptionKeys
{
    public class RenewSubscriptionKeysCommandHandler : IRequestHandler<RenewSubscriptionKeysCommand, Unit>
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IValidator<RenewSubscriptionKeysCommand> _validator;

        public RenewSubscriptionKeysCommandHandler(ISubscriptionService subscriptionService, IValidator<RenewSubscriptionKeysCommand> validator)
        {
            _subscriptionService = subscriptionService;
            _validator = validator;
        }
        
        public async Task<Unit> Handle(RenewSubscriptionKeysCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ValidationException(validationResult.DataAnnotationResult, null, null);
            }
            
            await _subscriptionService.RegenerateSubscriptionKeys(
                request.InternalUserId,
                Regex.IsMatch(request.InternalUserId, "^[0-9]+$") ? ApimUserType.Provider : ApimUserType.Employer,
                request.ProductName);

            return Unit.Value;
        }
    }
}