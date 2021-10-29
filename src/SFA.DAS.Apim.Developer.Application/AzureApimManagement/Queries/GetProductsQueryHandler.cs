using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, GetProductsQueryResponse>
    {
        private readonly IProductService _productService;

        public GetProductsQueryHandler (IProductService productService)
        {
            _productService = productService;
        }
        public async Task<GetProductsQueryResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var result = await _productService.GetProducts(request.Groups);

            return new GetProductsQueryResponse
            {
                Products = result
            };
        }
    }
}