using System;
using System.ComponentModel.DataAnnotations;
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
        private readonly IValidator<CreateUserCommand> _validator;

        public CreateUserCommandHandler (IUserService userService, IValidator<CreateUserCommand> validator)
        {
            _userService = userService;
            _validator = validator;
        }
        
        public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ValidationException(validationResult.DataAnnotationResult, null, null);
            }
            
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