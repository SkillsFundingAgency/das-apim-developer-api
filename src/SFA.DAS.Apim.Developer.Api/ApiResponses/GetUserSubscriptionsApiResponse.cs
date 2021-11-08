using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserSubscriptions;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Api.ApiResponses
{
    public class GetUserSubscriptionsApiResponse
    {
        public List<GetUserSubscriptionsApiResponseItem> Subscriptions { get; set; }
        public static implicit operator GetUserSubscriptionsApiResponse(GetUserSubscriptionsQueryResponse source)
        {
            return new GetUserSubscriptionsApiResponse
            {
                Subscriptions = source.UserSubscriptions.Select(c=>(GetUserSubscriptionsApiResponseItem)c).ToList()
            };
        }
    }

    public class GetUserSubscriptionsApiResponseItem
    {
        public string Key { get ; set ; }
        public string Name { get ; set ; }

        public static implicit operator GetUserSubscriptionsApiResponseItem(Subscription source)
        {
            return new GetUserSubscriptionsApiResponseItem
            {
                Name = source.Name,
                Key = source.PrimaryKey
            };
        }
    }
}