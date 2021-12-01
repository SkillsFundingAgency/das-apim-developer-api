using System;
using System.Net.Mail;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Validation;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUser
{
    public class CreateUserCommandValidator : IValidator<CreateUserCommand>
    {
        public Task<ValidationResult> ValidateAsync(CreateUserCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.Email))
            {
                validationResult.AddError(item.Email);
            }
            else
            {
                try
                {
                    var emailAddress = new MailAddress(item.Email);
                    if (!emailAddress.Address.Equals(item.Email, StringComparison.CurrentCultureIgnoreCase))
                    {
                        validationResult.AddError(nameof(item.Email));
                    }
                }
                catch (FormatException)
                {
                    validationResult.AddError(nameof(item.Email));
                }
            }

            if (string.IsNullOrEmpty(item.FirstName))
            {
                validationResult.AddError(nameof(item.LastName));
            }
            if (string.IsNullOrEmpty(item.LastName))
            {
                validationResult.AddError(nameof(item.LastName));
            }
            if (string.IsNullOrEmpty(item.Password))
            {
                validationResult.AddError(nameof(item.Password));
            }
            
            return Task.FromResult(validationResult);
        }
    }
}