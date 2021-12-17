using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Api.Controllers;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetProducts;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.Controllers.Products
{
    public class WhenGettingAvailableProducts
    {
        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Sent_And_Data_Returned(
            List<string> groups,
            GetProductsQueryResponse mediatorResponse,
            JObject documentation,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ProductsController controller)
        {
            foreach (var sourceProduct in mediatorResponse.Products)
            {
                sourceProduct.Documentation = JsonConvert.SerializeObject(documentation);
            }
            mediator.Setup(x => x.Send(It.Is<GetProductsQuery>(c => c.Groups.Equals(groups)), CancellationToken.None))
                .ReturnsAsync(mediatorResponse);

            var actual = await controller.GetProducts(groups) as OkObjectResult;

            actual.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var actualModel = actual.Value as GetProductsApiResponse;
            actualModel.Should().BeEquivalentTo((GetProductsApiResponse)mediatorResponse);
        }

        [Test, MoqAutoData]
        public async Task Then_If_An_Error_An_Internal_Server_Error_Is_Returned(
            List<string> groups,
            [Frozen] Mock<IMediator> mediator,
            [Greedy] ProductsController controller)
        {
            mediator.Setup(x => x.Send(It.IsAny<GetProductsQuery>(), CancellationToken.None))
                .ThrowsAsync(new Exception());

            var actual = await controller.GetProducts(groups) as StatusCodeResult;

            actual.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}