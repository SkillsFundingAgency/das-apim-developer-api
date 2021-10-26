using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureTokenService : IAzureTokenService
    {
        private const string AzureResource = "https://management.azure.com/";
        private readonly ChainedTokenCredential _azureServiceTokenProvider;

        public AzureTokenService(ChainedTokenCredential azureServiceTokenProvider)
        {
            _azureServiceTokenProvider = azureServiceTokenProvider;
        }
        public async Task<string> GetToken()
        {
            var token = await _azureServiceTokenProvider.GetTokenAsync(
                new TokenRequestContext(scopes: new string[] { AzureResource })
            );
            return token.Token;
        }
    }
}