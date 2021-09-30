using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimManagementService : IAzureApimManagementService
    {
        private readonly HttpClient _client;
        private readonly IOptions<AzureApimManagementConfiguration> _configuration;

        public AzureApimManagementService(HttpClient client, IOptions<AzureApimManagementConfiguration> configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task CreateSubscription()
        {
            throw new System.NotImplementedException();
        }
    }
}