using System.Collections.Generic;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserSubscriptions
{
    public class GetUserSubscriptionsQueryResponse
    {
        public IEnumerable<Subscription> UserSubscriptions { get ; set ; }
    }
}