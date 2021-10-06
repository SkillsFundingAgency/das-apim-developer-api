using System.Collections.Generic;

namespace SFA.DAS.Apim.Developer.Domain.Entities
{
    public class ApimUserType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ApimUser> ApimUsers { get; set; }
    }
}