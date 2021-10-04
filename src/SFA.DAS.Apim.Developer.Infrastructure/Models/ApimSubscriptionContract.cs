using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;


namespace SFA.DAS.Apim.Developer.Infrastructure.Models
{
    //https://docs.microsoft.com/en-us/rest/api/apimanagement/2021-04-01-preview/subscription/create-or-update#subscriptioncontract
    public class ApimSubscriptionContract
    {
        public string displayName { get; set; }
        public string scope { get; set; }
        public string ownerId { get; set; }
        public SubscriptionState state { get; set; }

        public string primaryKey { get; set; }
        public string secondaryKey { get; set; }
    }

    public enum SubscriptionState
    {
        active,
        cancelled,
        expired,
        rejected,
        submitted,
        suspended
    }
}