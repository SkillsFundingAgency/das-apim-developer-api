using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Data.Repository
{
    public class ApimUserRepository : IApimUserRepository
    {
        private readonly IApimDeveloperDataContext _apimDeveloperDataContext;

        public ApimUserRepository(IApimDeveloperDataContext apimDeveloperDataContext)
        {
            _apimDeveloperDataContext = apimDeveloperDataContext;
        }

        public async Task<ApimUser> Insert(ApimUser apimUser)
        {
            await _apimDeveloperDataContext.ApimUser.AddAsync(apimUser);
            await _apimDeveloperDataContext.SaveChangesAsync();
            return apimUser;
        }

        public async Task<ApimUser> GetByInternalIdAndType(string internalUserId, int apimUserTypeId)
        {
            var apimUser = await _apimDeveloperDataContext
                .ApimUser
                .SingleOrDefaultAsync(a => a.InternalUserId == internalUserId && a.ApimUserTypeId == apimUserTypeId);
            return apimUser;
        }

        public async Task<IEnumerable<ApimUser>> GetAll()
        {
            var apimUsers = await _apimDeveloperDataContext
                .ApimUser
                .ToListAsync();

            return apimUsers;
        }
    }
}
