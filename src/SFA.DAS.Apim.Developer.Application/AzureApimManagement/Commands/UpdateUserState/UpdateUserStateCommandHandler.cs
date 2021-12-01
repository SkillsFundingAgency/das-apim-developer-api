using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.UpdateUserState
{
    public class UpdateUserStateCommandHandler : IRequestHandler<UpdateUserStateCommand, Unit>
    {
        private readonly IUserService _userService;

        public UpdateUserStateCommandHandler (IUserService userService)
        {
            _userService = userService;
        }
        
        public async Task<Unit> Handle(UpdateUserStateCommand request, CancellationToken cancellationToken)
        {
            await _userService.UpdateUserState(request.UserId);
            
            return Unit.Value;
        }
    }
}