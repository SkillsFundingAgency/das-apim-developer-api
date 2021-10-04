using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Infrastructure.Models
{
    public class AzureResourcesRequest : IGetRequest
    {
        public string GetUrl { get; set; }
    }
}