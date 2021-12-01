using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using SFA.DAS.Apim.Developer.Api.ApiRequests;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUser;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.UpdateUserState;

namespace SFA.DAS.Apim.Developer.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController (IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateUser(CreateUserApiRequest request)
        {
            try
            {
                await _mediator.Send(new CreateUserCommand
                {
                    Email = request.Email,
                    Password = request.Password,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                });
                return Created("", null);
            }
            catch (ValidationException e)
            {
                return BadRequest(e.ValidationResult.ErrorMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUserState(string id)
        {
            try
            {
                await _mediator.Send(new UpdateUserStateCommand
                {
                    UserId = id
                });
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}