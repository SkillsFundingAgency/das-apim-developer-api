using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Validation;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateSubscription
{
    public class CreateSubscriptionCommandValidator : IValidator<CreateSubscriptionCommand>
    {
        public Task<ValidationResult> ValidateAsync(CreateSubscriptionCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.InternalUserId))
            {
                validationResult.AddError(nameof(item.InternalUserId));
            }

            if (string.IsNullOrEmpty(item.ProductName))
            {
                validationResult.AddError(nameof(item.ProductName));
            }

            return Task.FromResult(validationResult);
        }
    }
}