using System;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class ApimAudit
    {
        public int Id { get; set; }
        public Guid ApimUserId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual ApimUser ApimUser { get; set; }
    }
}