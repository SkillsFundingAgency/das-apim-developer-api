using System.Net.Http;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Configuration;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimManagementService : AzureApimHttpClientServiceBase, IAzureApimManagementService
    {

        public AzureApimManagementService(HttpClient client, IAzureTokenService azureTokenService, AzureApimManagementConfiguration azureApimManagementConfiguration) 
            : base(azureTokenService,client,$"https://management.azure.com/{azureApimManagementConfiguration.ApimResourceId}/")
        {
        
        }

    }

    public class AzureUserAuthenticationManagementService : AzureApimHttpClientServiceBase,
        IAzureUserAuthenticationManagementService
    {
        public AzureUserAuthenticationManagementService(IAzureTokenService azureTokenService, HttpClient httpClient, AzureApimManagementConfiguration azureApimManagementConfiguration) 
            : base(azureTokenService, httpClient, $"{azureApimManagementConfiguration.ApimUserManagementUrl}{azureApimManagementConfiguration.ApimResourceId}/")
        {
        }
    }
}