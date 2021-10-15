using System.Threading.Tasks;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IAzureTokenService
    {
        Task<string> GetToken();
    }
}