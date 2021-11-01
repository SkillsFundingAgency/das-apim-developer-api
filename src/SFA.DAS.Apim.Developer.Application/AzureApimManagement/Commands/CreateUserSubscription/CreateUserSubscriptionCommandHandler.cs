using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;

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
                throw new ValidationException(validationResult.DataAnnotationResult, null, null);
            }

            var subscriptionResponse = await _subscriptionService.CreateUserSubscription(
                request.InternalUserId,
                Regex.IsMatch(request.InternalUserId, "^[0-9]+$") ? ApimUserType.Provider : ApimUserType.Employer,
                request.ProductName,
                request.UserDetails);

            return new CreateUserSubscriptionCommandResponse
            {
                SubscriptionId = subscriptionResponse.Name,
                PrimaryKey = subscriptionResponse.PrimaryKey,
                SandboxPrimaryKey = subscriptionResponse.SandboxPrimaryKey
            };
        }
    }
}