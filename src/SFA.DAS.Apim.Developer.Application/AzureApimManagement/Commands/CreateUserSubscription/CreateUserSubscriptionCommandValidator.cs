using System;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Validation;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription
{
    public class CreateUserSubscriptionCommandValidator : IValidator<CreateUserSubscriptionCommand>
    {
        public Task<ValidationResult> ValidateAsync(CreateUserSubscriptionCommand item)
        {
            var validationResult = new ValidationResult();
            
            if (string.IsNullOrEmpty(item.ApimUserType))
            {
                validationResult.AddError(nameof(item.ApimUserType));
            }
            
            if (string.IsNullOrEmpty(item.InternalUserId))
            {
                validationResult.AddError(nameof(item.InternalUserId));
            }
            
            if (string.IsNullOrEmpty(item.ProductName))
            {
                validationResult.AddError(nameof(item.ProductName));
            }

            if (item.ApimUserId.Equals(Guid.Empty))
            {
                validationResult.AddError(nameof(item.ApimUserId));
            }
            
            return Task.FromResult(validationResult);
        }
    }
}