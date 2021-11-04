using MediatR;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateSubscription
{
    public class CreateSubscriptionCommand : IRequest<CreateSubscriptionCommandResponse>
    {
        public string InternalUserId { get; set; }
        public string ProductName { get; set; }
    }
}