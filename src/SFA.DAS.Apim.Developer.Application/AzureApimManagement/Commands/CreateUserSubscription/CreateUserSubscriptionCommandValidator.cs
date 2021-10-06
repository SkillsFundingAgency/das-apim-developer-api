using System;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Validation;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription
{
    public class CreateUserSubscriptionCommandValidator : IValidator<CreateUserSubscriptionCommand>
    {
        public async Task<ValidationResult> ValidateAsync(CreateUserSubscriptionCommand item)
        {
            //TODO: Validate input
            return new ValidationResult();
        }
    }
}