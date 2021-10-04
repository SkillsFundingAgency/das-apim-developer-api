using System.Collections.Generic;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Infrastructure.Models
{
    public class AzureSubscriptionsRequest : IGetRequest
    {
        public string GetUrl { get; set; }
    }
}