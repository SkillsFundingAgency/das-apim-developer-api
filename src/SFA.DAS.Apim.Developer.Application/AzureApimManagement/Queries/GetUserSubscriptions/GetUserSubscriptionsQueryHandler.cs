using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Extensions;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserSubscriptions
{
    public class GetUserSubscriptionsQueryHandler : IRequestHandler<GetUserSubscriptionsQuery, GetUserSubscriptionsQueryResponse>
    {
        private readonly ISubscriptionService _subscriptionService;

        public GetUserSubscriptionsQueryHandler (ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        public async Task<GetUserSubscriptionsQueryResponse> Handle(GetUserSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var result = await _subscriptionService.GetUserSubscriptions(request.InternalUserId,
                request.InternalUserId.ApimUserType());

            return new GetUserSubscriptionsQueryResponse
            {
                UserSubscriptions = result
            };
        }
    }
}