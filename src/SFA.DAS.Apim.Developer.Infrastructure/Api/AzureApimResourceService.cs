using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Infrastructure.Models;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimResourceService : IAzureApimResourceService
    {
        private readonly IAzureApimManagementService _azureApimManagementService;
        private readonly IOptions<AzureApimManagementConfiguration> _configuration;

        public AzureApimResourceService(AzureApimManagementService azureApimManagementService, IOptions<AzureApimManagementConfiguration> configuration)
        {
            _azureApimManagementService = azureApimManagementService;
            _configuration = configuration;
        }
        public async Task<string> GetResourceId()
        {
            var subsRequest = new AzureSubscriptionsRequest
            {
                GetUrl = $"/subscriptions?api-version=2020-01-01"
            };
            var subs = await _azureApimManagementService.Get<AzureSubscriptionsResponse>(subsRequest);
            if (subs.value.Count != 1)
            {
                throw new System.Exception("Subscription count unexpected");
            }
            var subscriptionId = subs.value.First().subscriptionId;

            var apimRequest = new AzureResourcesRequest
            {
                GetUrl = $"/subscriptions/{subscriptionId}/resources?$filter=resourceType eq 'Microsoft.ApiManagement/service' and name eq '{_configuration.Value.ApimServiceName}'&api-version=2021-04-01"
            };
            var apimResources = await _azureApimManagementService.Get<AzureResourcesReponse>(apimRequest);
            if (apimResources.value.Count != 1)
            {
                throw new System.Exception("Apim Resources count unexpected");
            }

            return apimResources.value.First().id;
        }
    }
}