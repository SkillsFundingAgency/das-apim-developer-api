using System;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class ApimSubscriptionAudit
    {
        public long Id { get; set; }
        public Guid UserId { get; set; }
        public string ProductName { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }

    }
}