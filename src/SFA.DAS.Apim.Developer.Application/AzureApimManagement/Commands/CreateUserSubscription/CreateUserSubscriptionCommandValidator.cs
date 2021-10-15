using System;
using System.Net.Mail;
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

            if (string.IsNullOrEmpty(item.InternalUserId))
            {
                validationResult.AddError(nameof(item.InternalUserId));
            }

            if (string.IsNullOrEmpty(item.ProductName))
            {
                validationResult.AddError(nameof(item.ProductName));
            }

            if (string.IsNullOrEmpty(item.UserDetails.FirstName))
            {
                validationResult.AddError(nameof(item.UserDetails.FirstName));

            }

            if (string.IsNullOrEmpty(item.UserDetails.LastName))
            {
                validationResult.AddError(nameof(item.UserDetails.LastName));
            }

            if (string.IsNullOrEmpty(item.UserDetails.EmailAddress))
            {
                validationResult.AddError(nameof(item.UserDetails.EmailAddress));
            }
            else
            {
                try
                {
                    var emailAddress = new MailAddress(item.UserDetails.EmailAddress);
                    if (!emailAddress.Address.Equals(item.UserDetails.EmailAddress, StringComparison.CurrentCultureIgnoreCase))
                    {
                        validationResult.AddError(nameof(item.UserDetails.EmailAddress));
                    }
                }
                catch (FormatException)
                {
                    validationResult.AddError(nameof(item.UserDetails.EmailAddress));
                }
            }
            return Task.FromResult(validationResult);
        }
    }
}