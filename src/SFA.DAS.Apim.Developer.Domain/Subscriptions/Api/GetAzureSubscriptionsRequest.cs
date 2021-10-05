using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Domain.Subscriptions.Api
{
    public class GetAzureSubscriptionsRequest : IGetRequest
    {
        public string GetUrl => "subscriptions?api-version=2020-01-01";
    }
}