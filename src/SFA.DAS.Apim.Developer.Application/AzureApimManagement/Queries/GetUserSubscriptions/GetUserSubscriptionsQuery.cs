using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserSubscriptions
{
    public class GetUserSubscriptionsQuery : IRequest<GetUserSubscriptionsQueryResponse>
    {
        public string InternalUserId { get ; set ; }
    }
}