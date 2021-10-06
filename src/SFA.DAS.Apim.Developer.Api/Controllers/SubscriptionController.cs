using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apim.Developer.Api.ApiRequests;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription;

namespace SFA.DAS.Apim.Developer.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController (IMediator mediator, ILogger<SubscriptionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> CreateSubscription([FromRoute]Guid id, [FromBody]CreateSubscriptionApiRequest request)
        {
            try
            {
                var queryResult = await _mediator.Send(new CreateUserSubscriptionCommand
                {
                    ProductName = request.ProductId,
                    InternalUserId = request.AccountIdentifier,
                    ApimUserId = id
                });

                return Created("", new {Id = queryResult.SubscriptionId});
            }
            catch (ValidationException e)
            {
                return BadRequest(e.ValidationResult.ErrorMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }
    }
}