using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IAzureUserAuthenticationManagementService
    {
        Task<ApiResponse<T>> GetAuthentication<T>(IGetUserAuthenticationRequest getUserAuthenticationRequest, string requestEncoding = "application/json");
    }
}