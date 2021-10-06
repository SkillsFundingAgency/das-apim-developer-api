using System;
using System.Collections.Generic;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class ApimUser
    {
        public Guid ApimUserId { get; set; }
        public string InternalUserId { get; set; }
        public int ApimUserTypeId { get; set; }

        public virtual ApimUserType ApimUserType { get; set; }
        public virtual ICollection<ApimAudit> ApimAudits { get; set; }
    }
}