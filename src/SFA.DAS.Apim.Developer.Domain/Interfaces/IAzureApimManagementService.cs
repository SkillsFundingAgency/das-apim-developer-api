using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IAzureApimManagementService
    {
        Task<ApiResponse<T>> Put<T>(IPutRequest putRequest);
        Task<ApiResponse<T>> Get<T>(IGetRequest getRequest);
        Task<ApiResponse<T>> Post<T>(IPostRequest getRequest);
    }
}