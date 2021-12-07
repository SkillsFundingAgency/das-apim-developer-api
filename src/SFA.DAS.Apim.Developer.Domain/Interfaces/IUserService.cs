using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IUserService
    {
        Task<UserDetails> CreateUser(UserDetails userDetails);
        Task<UserDetails> GetUser(string emailAddress);
        Task<UserDetails> CheckUserAuthentication(string email, string password);
        Task<UserDetails> UpdateUser(UserDetails userDetails);
    }
}