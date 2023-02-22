using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.DeleteSubscription
{
    public class DeleteSubscriptionCommand : IRequest<Unit>
    {
        public string InternalUserId { get; set; }
        public string ProductName { get; set; }
    }
}