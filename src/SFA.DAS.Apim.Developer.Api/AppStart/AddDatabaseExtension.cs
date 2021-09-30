using System;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apim.Developer.Data;
using SFA.DAS.Apim.Developer.Domain.Configuration;

namespace SFA.DAS.Apim.Developer.Api.AppStart
{
    public static class AddDatabaseExtension
    {
        public static void AddDatabaseRegistration(this IServiceCollection services, ApimDeveloperApiConfiguration config, string environmentName)
        {
            if (environmentName.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ApimDeveloperDataContext>(options => options.UseInMemoryDatabase("SFA.DAS.Apim.Developer"), ServiceLifetime.Transient);
            }
            else if (environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddDbContext<ApimDeveloperDataContext>(options => options.UseSqlServer(config.ConnectionString), ServiceLifetime.Transient);
            }
            else
            {
                services.AddSingleton(new AzureServiceTokenProvider());
                services.AddDbContext<ApimDeveloperDataContext>(ServiceLifetime.Transient);
            }



            services.AddTransient<IApimDeveloperDataContext, ApimDeveloperDataContext>(provider => provider.GetService<ApimDeveloperDataContext>());
            services.AddTransient(provider => new Lazy<ApimDeveloperDataContext>(provider.GetService<ApimDeveloperDataContext>()));

        }
    }
}
