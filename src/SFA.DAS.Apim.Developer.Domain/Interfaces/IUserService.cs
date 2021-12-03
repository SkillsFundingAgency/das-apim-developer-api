using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IUserService
    {
        Task<UserDetails> UpsertUser(UserDetails userDetails);
        Task<UserDetails> GetUser(string emailAddress);
        Task UpdateUserState(string email);
        Task<UserDetails> CheckUserAuthentication(string email, string password);
    }
}