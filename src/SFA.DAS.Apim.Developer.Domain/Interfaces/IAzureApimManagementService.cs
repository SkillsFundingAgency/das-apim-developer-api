using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IAzureApimManagementService
    {
        Task<ApiResponse<T>> Put<T>(IPutRequest putRequest);
        Task<ApiResponse<T>> Get<T>(IGetRequest getRequest, string requestEncoding = "application/json");
        Task<ApiResponse<T>> Post<T>(IPostRequest getRequest);
        Task<ApiResponse<T>> GetAuthentication<T>(IGetUserAuthenticationRequest getUserAuthenticationRequest, string requestEncoding = "application/json");
    }
}