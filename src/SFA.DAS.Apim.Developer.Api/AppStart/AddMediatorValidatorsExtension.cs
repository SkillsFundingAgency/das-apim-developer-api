using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateSubscription;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUser;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.DeleteSubscription;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.RenewSubscriptionKeys;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Api.AppStart
{
    public static class AddMediatorValidatorsExtension
    {
        public static void AddMediatorValidators(this IServiceCollection services)
        {
            services.AddTransient<IValidator<CreateUserCommand>, CreateUserCommandValidator>();
            services.AddTransient<IValidator<CreateSubscriptionCommand>, CreateSubscriptionCommandValidator>();
            services.AddTransient<IValidator<RenewSubscriptionKeysCommand>, RenewSubscriptionKeysCommandValidator>();
            services.AddTransient<IValidator<DeleteSubscriptionCommand>, DeleteSubscriptionCommandValidator>();
        }
    }
}