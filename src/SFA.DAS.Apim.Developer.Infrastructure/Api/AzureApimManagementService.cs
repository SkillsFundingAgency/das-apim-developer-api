using System.Net.Http;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Configuration;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimManagementService : AzureApimManagementServiceBase, IAzureApimManagementService
    {

        public AzureApimManagementService(HttpClient client, IAzureTokenService azureTokenService, ApimResourceConfiguration resourceConfiguration) 
            : base(azureTokenService,client,$"https://management.azure.com/{resourceConfiguration.ApimResourceId}/")
        {
        
        }

    }
}