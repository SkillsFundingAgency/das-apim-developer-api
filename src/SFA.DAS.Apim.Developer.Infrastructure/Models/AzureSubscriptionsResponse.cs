using System.Collections.Generic;


namespace SFA.DAS.Apim.Developer.Infrastructure.Models
{
    //https://docs.microsoft.com/en-us/rest/api/resources/subscriptions/list
    public class AzureSubscriptionsResponse
    {


        public class AzureSubscription
        {
            public string displayName { get; set; }
            public string id { get; set; }
            public string subscriptionId { get; set; }
        }

        public List<AzureSubscription> value { get; set; }
    }
}