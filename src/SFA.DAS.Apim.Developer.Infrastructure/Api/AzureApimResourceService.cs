using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses;
using SFA.DAS.Apim.Developer.Infrastructure.Models;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimResourceService : AzureApimHttpClientServiceBase, IAzureApimResourceService
    {

        private readonly AzureApimManagementConfiguration _configuration;

        public AzureApimResourceService(HttpClient client, IAzureTokenService azureTokenService, IOptions<AzureApimManagementConfiguration> configuration) :
            base(azureTokenService, client, "https://management.azure.com/")
        {
            _configuration = configuration.Value;
        }

        public async Task<string> GetResourceId()
        {
            var apimResources = await Post<AzureResourcesResponse>(new ListAzureApimResourcesRequest(_configuration.ApimServiceName));

            if (apimResources.Body.TotalRecords != 1)
            {
                throw new Exception("Apim Resources count unexpected");
            }

            return apimResources.Body.AzureResources.First().Id;
        }
    }
}