using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureTokenService : IAzureTokenService
    {
        private const string AzureResource = "https://management.azure.com/";
        private readonly AzureServiceTokenProvider _azureServiceTokenProvider;

        public AzureTokenService (AzureServiceTokenProvider azureServiceTokenProvider)
        {
            _azureServiceTokenProvider = azureServiceTokenProvider;
        }
        public async Task<string> GetToken()
        {
            var token = await _azureServiceTokenProvider.GetAccessTokenAsync(AzureResource);
            return token;
        }
    }
}