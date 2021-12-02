using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserAuthenticated
{
    public class GetUserAuthenticatedQuery : IRequest<GetUserAuthenticatedQueryResponse>
    {
        public string Email { get ; set ; }
        public string Password { get ; set ; }
    }
}