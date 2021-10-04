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
        [TestCase(typeof(IAzureTokenService))]
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

            foreach (var descriptor in serviceCollection.Where(
                d => d.ServiceType ==
                    typeof(IAzureApimResourceService)).ToList())
            {
                serviceCollection.Remove(descriptor);
            }
            serviceCollection.AddSingleton(Mock.Of<IAzureApimResourceService>());

            var provider = serviceCollection.BuildServiceProvider();

            var type = provider.GetService(toResolve);
            Assert.IsNotNull(type);
        }

        [Test]
        public void Then_The_ApimResourceId_Is_Set()
        {
            var hostEnvironment = new Mock<IWebHostEnvironment>();
            var serviceCollection = new ServiceCollection();
            var azureApimResourceService = new Mock<IAzureApimResourceService>();
            azureApimResourceService.Setup(x => x.GetResourceId()).ReturnsAsync("test");
            var configuration = GenerateConfiguration();
            serviceCollection.AddSingleton(hostEnvironment.Object);
            serviceCollection.AddSingleton(Mock.Of<IConfiguration>());
            serviceCollection.AddConfigurationOptions(configuration);
            serviceCollection.AddDistributedMemoryCache();
            serviceCollection.AddServiceRegistration();
            foreach (var descriptor in serviceCollection.Where(
                d => d.ServiceType ==
                     typeof(IAzureApimResourceService)).ToList())
            {
                serviceCollection.Remove(descriptor);
            }
            serviceCollection.AddSingleton(azureApimResourceService.Object);
            var provider = serviceCollection.BuildServiceProvider();

            var type = provider.GetService<ApimResourceConfiguration>();
            type.ApimResourceId.Should().Be("test");
        }

        private static IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("ApimDeveloperApi:ConnectionString", "test"),
                    new KeyValuePair<string, string>("AzureApimManagement:ApimServiceName", "test")
                }
            };

            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
    }
}