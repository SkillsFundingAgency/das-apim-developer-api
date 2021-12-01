using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Unit>
    {
        private readonly IUserService _userService;

        public CreateUserCommandHandler (IUserService userService)
        {
            _userService = userService;
        }
        
        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            await _userService.CreateUser(new UserDetails
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            });
            return Unit.Value;
        }
    }
}