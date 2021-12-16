using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.AppStart;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.AppStart
{
    public class WhenAddingServicesToTheContainer
    {

        [TestCase(typeof(IAzureApimManagementService))]
        [TestCase(typeof(IAzureUserAuthenticationManagementService))]
        [TestCase(typeof(IAzureTokenService))]
        [TestCase(typeof(ISubscriptionService))]
        [TestCase(typeof(IUserService))]
        [TestCase(typeof(IApimUserRepository))]
        [TestCase(typeof(IApimAuditRepository))]
        [TestCase(typeof(IProductService))]
        [TestCase(typeof(IApimSubscriptionAuditRepository))]
        public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
        {
            var hostEnvironment = new Mock<IWebHostEnvironment>();
            var serviceCollection = new ServiceCollection();

            var configuration = GenerateConfiguration();
            serviceCollection.AddSingleton(hostEnvironment.Object);
            serviceCollection.AddSingleton(Mock.Of<IConfiguration>());
            serviceCollection.AddConfigurationOptions(configuration);
            serviceCollection.AddDistributedMemoryCache();
            serviceCollection.AddServiceRegistration();

            var apimDeveloperApiConfiguration = configuration
                .GetSection("ApimDeveloperApi")
                .Get<ApimDeveloperApiConfiguration>();
            serviceCollection.AddDatabaseRegistration(apimDeveloperApiConfiguration, configuration["Environment"]);

            var provider = serviceCollection.BuildServiceProvider();

            var type = provider.GetService(toResolve);
            Assert.IsNotNull(type);
        }

        private static IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("ApimDeveloperApi:ConnectionString", "test"),
                    new KeyValuePair<string, string>("AzureApimManagement:ApimServiceName", "test"),
                    new KeyValuePair<string, string>("AzureApimManagement:ApimUserManagementUrl", "https://test/"),
                    new KeyValuePair<string, string>("Environment", "test"),
                }
            };

            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
    }
}