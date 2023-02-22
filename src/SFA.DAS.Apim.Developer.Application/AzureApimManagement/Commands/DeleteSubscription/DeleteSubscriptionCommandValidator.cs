using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Validation;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.DeleteSubscription
{
    public class DeleteSubscriptionCommandValidator : IValidator<DeleteSubscriptionCommand>
    {
        public Task<ValidationResult> ValidateAsync(DeleteSubscriptionCommand item)
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