using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apim.Developer.Api.ApiRequests;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserSubscriptions;

namespace SFA.DAS.Apim.Developer.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(IMediator mediator, ILogger<SubscriptionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionApiRequest request)
        {
            try
            {
                var queryResult = await _mediator.Send(new CreateUserSubscriptionCommand
                {
                    ProductName = request.ProductId,
                    InternalUserId = request.AccountIdentifier,
                    UserDetails = new Domain.Models.UserDetails
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        EmailAddress = request.EmailAddress
                    }
                });

                var response = (CreateSubscriptionApiResponse)queryResult;

                return Created("", response);
            }
            catch (ValidationException e)
            {
                return BadRequest(e.ValidationResult.ErrorMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task <IActionResult> GetUserSubscriptions([FromRoute] string id)
        {
            try
            {
                var result = await _mediator.Send(new GetUserSubscriptionsQuery
                {
                    InternalUserId = id
                });
            
                return Ok((GetUserSubscriptionsApiResponse)result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}