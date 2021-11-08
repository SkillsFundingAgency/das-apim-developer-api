using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetProducts
{
    public class GetProductsQuery : IRequest<GetProductsQueryResponse>
    {
        public List<string> Groups { get ; set ; }
    }
}