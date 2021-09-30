using System;
using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription
{
    public class CreateUserSubscriptionCommand : IRequest<CreateUserSubscriptionCommandResponse>
    {
        public Guid Id { get ; set ; }
        public string ExternalSubscriberId { get ; set ; }
        public int ExternalSubscriptionId { get ; set ; }
        public int SubscriberTypeId { get ; set ; }
    }
}