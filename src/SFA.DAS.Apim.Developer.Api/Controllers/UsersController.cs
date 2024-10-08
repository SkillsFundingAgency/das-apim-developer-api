using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apim.Developer.Api.ApiRequests;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUser;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.UpdateUserState;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserAuthenticated;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserByEmail;

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
        [Route("{id}")]
        public async Task<IActionResult> CreateUser([FromRoute]string id, UpsertUserApiRequest request)
        {
            try
            {
                var actual = await _mediator.Send(new CreateUserCommand
                {
                    Id = id,
                    Email = request.Email,
                    Password = request.Password,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    State = request.State.ToString(),
                    ConfirmEmailLink = request.ConfirmEmailLink
                });
                return Created("", new {id=actual});
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
        public async Task<IActionResult> UpsertUser([FromRoute]string id, UpsertUserApiRequest request)
        {
            try
            {
                var result = await _mediator.Send(new UpdateUserCommand
                {
                    Id = id,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    State = request.State?.ToString(),
                    Password = request.Password,
                    ConfirmEmailLink = request.ConfirmEmailLink
                });

                if (result.UserDetails == null)
                {
                    return NotFound();
                }
                
                return NoContent();
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

        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> AuthenticateUser(AuthenticateRequest request)
        {
            try
            {
                var result = await _mediator.Send(new GetUserAuthenticatedQuery
                {
                    Email = request.Email,
                    Password = request.Password
                });

                if (result.User == null)
                {
                    return Unauthorized();
                }

                return Ok((GetUserApiResponse)result.User);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
        
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetUser([FromQuery]string email)
        {
            try
            {
                var result = await _mediator.Send(new GetUserByEmailQuery
                {
                    Email = email
                });

                if (result.User == null)
                {
                    return NotFound();
                }

                return Ok((GetUserApiResponse)result.User);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}