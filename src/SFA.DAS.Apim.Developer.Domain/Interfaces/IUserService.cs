using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IUserService
    {
        Task<UserDetails> CreateUser(UserDetails userDetails);
        Task<UserDetails> GetUser(string emailAddress);
        Task UpdateUserState(string userId);
    }
}