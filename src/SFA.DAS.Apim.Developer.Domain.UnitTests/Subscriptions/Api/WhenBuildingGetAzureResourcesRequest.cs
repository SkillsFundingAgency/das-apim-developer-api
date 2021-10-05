using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingGetAzureResourcesRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string subscriptionId, string apimServiceName)
        {
            var actual = new GetAzureResourcesRequest(subscriptionId, apimServiceName);

            actual.GetUrl.Should()
                .Be($"subscriptions/{subscriptionId}/resources?$filter=resourceType eq 'Microsoft.ApiManagement/service' and name eq '{apimServiceName}'&api-version=2021-04-01");
        }
    }
}