using System;
using System.Collections.Generic;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class ApimUser
    {
        public Guid Id { get; set; }
        public string InternalUserId { get; set; }
        public int ApimUserTypeId { get; set; }
        public string ApimUserId { get; set; }

        public virtual ICollection<ApimAudit> ApimAudits { get; set; }
    }
}