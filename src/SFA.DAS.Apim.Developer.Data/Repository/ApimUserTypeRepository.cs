using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Data.Repository
{
    public class ApimUserTypeRepository : IApimUserTypeRepository
    {
        private readonly IApimDeveloperDataContext _apimDeveloperDataContext;

        public ApimUserTypeRepository(IApimDeveloperDataContext apimDeveloperDataContext)
        {
            _apimDeveloperDataContext = apimDeveloperDataContext;
        }

        public async Task<ApimUserType> Get(string name)
        {
            var apimUserType = await _apimDeveloperDataContext
                .ApimUserType
                .SingleOrDefaultAsync(c => c.Name.Equals(name));
            return apimUserType;
        }

        public async Task<IEnumerable<ApimUserType>> GetAll()
        {
            var apimUserTypes = await _apimDeveloperDataContext
                .ApimUserType
                .ToListAsync();

            return apimUserTypes;
        }
    }
}
