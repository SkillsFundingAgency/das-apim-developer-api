using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Validation;

namespace SFA.DAS.Apim.Developer.Domain.Interfaces
{
    public interface IValidator<T>
    {
        Task<ValidationResult> ValidateAsync(T item);
    }
}