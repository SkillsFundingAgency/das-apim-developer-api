using System.Collections.Generic;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class SubscriberType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}