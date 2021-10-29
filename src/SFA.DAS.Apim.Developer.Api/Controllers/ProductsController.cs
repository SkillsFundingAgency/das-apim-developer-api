using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries;

namespace SFA.DAS.Apim.Developer.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController (IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetProducts([FromQuery] List<string> group)
        {
            try
            {
                var result = await _mediator.Send(new GetProductsQuery
                {
                    Groups = group
                });
            
                return Ok((GetProductsApiResponse)result);
            }
            catch (Exception e)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
            
        }
    }
}