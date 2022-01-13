using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserByEmail
{
    public class GetUserByEmailQuery : IRequest<GetUserByEmailQueryResponse>
    {
        public string Email { get; set; }
    }
}