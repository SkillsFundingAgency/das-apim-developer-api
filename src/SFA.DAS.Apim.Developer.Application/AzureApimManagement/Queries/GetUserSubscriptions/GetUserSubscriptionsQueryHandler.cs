using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;

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
                Regex.IsMatch(request.InternalUserId, "^[0-9]+$") ? ApimUserType.Provider : ApimUserType.Employer);

            return new GetUserSubscriptionsQueryResponse
            {
                UserSubscriptions = result
            };
        }
    }
}