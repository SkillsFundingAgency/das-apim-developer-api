using System;
using System.Text.RegularExpressions;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Extensions
{
    public static class ApimUserTypeExtension
    {
        public static ApimUserType ApimUserType(this string id)
        {
            if (Regex.IsMatch(id, "^[0-9]+$"))
            {
                return Models.ApimUserType.Provider;
            }

            if (Guid.TryParse(id, out _))
            {
                return Models.ApimUserType.External;
            }
            
            return Models.ApimUserType.Employer;
        }
    }
}