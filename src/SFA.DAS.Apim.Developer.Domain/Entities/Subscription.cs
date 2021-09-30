using System;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public int ExternalSubscriptionId { get; set; }
        public string ExternalSubscriberId { get; set; }
        public int SubscriberTypeId { get; set; }
    }
}