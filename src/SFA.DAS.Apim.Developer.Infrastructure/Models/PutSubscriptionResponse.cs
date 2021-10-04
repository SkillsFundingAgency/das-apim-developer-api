using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Apim.Developer.Domain.Interfaces;


namespace SFA.DAS.Apim.Developer.Infrastructure.Models
{
    //https://docs.microsoft.com/en-us/rest/api/apimanagement/2021-04-01-preview/subscription/create-or-update#subscriptioncontract
    public class PutSubscriptionResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public ApimSubscriptionContract properties { get; set; }
    }
}