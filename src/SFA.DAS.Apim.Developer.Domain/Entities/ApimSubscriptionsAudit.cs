using System;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class ApimSubscriptionsAudit
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string ProductName { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }

    }
}