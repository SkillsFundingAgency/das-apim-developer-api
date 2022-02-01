using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserByEmail
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, GetUserByEmailQueryResponse>
    {
        private readonly IUserService _userService;

        public GetUserByEmailQueryHandler(IUserService userService)
        {
            _userService = userService;
        }
        
        public async Task<GetUserByEmailQueryResponse> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUser(request.Email);

            return new GetUserByEmailQueryResponse
            {
                User = user
            };
        }
    }
}