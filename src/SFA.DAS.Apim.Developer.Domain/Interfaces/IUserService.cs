using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IUserService
    {
        Task<string> CreateUser(string internalUserId, UserDetails userDetails, ApimUserType apimUserType);
    }
}