using System;
using System.Collections.Generic;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public int ExternalSubscriptionId { get; set; }
        public string ExternalSubscriberId { get; set; }
        public int SubscriberTypeId { get; set; }

        public virtual SubscriberType SubscriberType { get; set; }
        public virtual ICollection<SubscriptionAudit> SubscriptionAudits { get; set; }
    }
}