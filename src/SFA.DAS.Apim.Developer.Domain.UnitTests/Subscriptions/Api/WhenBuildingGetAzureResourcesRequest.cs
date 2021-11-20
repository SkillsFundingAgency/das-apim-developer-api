using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Subscriptions.Api
{
    public class WhenBuildingListAzureApimResourcesRequest
    {
        [Test, AutoData]
        public void Then_The_Url_Is_Correctly_Constructed(string apimServiceName)
        {
            var actual = new ListAzureApimResourcesRequest(apimServiceName);

            actual.PostUrl.Should()
                .Be("providers/Microsoft.ResourceGraph/resources?api-version=2021-03-01");
        }

        [Test, AutoData]
        public void Then_The_Data_Is_Correctly_Set(string apimServiceName)
        {
            var expectedQuery = $"where name=~'{apimServiceName}' and type=~'microsoft.apimanagement/service'";

            var actual = new ListAzureApimResourcesRequest(apimServiceName);

            ((ListAzureApimResourcesRequestBody)actual.Data).Query.Should().BeEquivalentTo(expectedQuery);
        }
    }
}