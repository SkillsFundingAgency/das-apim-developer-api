using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserAuthenticated
{
    public class GetUserAuthenticatedQueryHandler : IRequestHandler<GetUserAuthenticatedQuery, GetUserAuthenticatedQueryResponse>
    {
        private readonly IUserService _userService;

        public GetUserAuthenticatedQueryHandler (IUserService userService)
        {
            _userService = userService;
        }
        public async Task<GetUserAuthenticatedQueryResponse> Handle(GetUserAuthenticatedQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.CheckUserAuthentication(request.Email, request.Password);

            return new GetUserAuthenticatedQueryResponse
            {
                User = user
            };
        }
    }
}