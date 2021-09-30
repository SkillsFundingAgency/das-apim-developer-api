using System;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class SubscriptionAudit
    {
        public int Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public string UserRef { get; set; }
        public string Action { get; set; }

        public virtual Subscription Subscription { get; set; }
    }
}