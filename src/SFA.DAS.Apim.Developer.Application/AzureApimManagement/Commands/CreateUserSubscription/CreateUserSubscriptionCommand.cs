using MediatR;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription
{
    public class CreateUserSubscriptionCommand : IRequest<CreateUserSubscriptionCommandResponse>
    {
        public string InternalUserId { get; set; }
        public string ProductName { get; set; }
        public UserDetails UserDetails { get; set; }
    }
}