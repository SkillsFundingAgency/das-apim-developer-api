using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateSubscription;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Api.AppStart
{
    public static class AddMediatorValidatorsExtension
    {
        public static void AddMediatorValidators(this IServiceCollection services)
        {
            services.AddTransient<IValidator<CreateSubscriptionCommand>, CreateSubscriptionCommandValidator>();
        }
    }
}