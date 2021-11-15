using MediatR;

namespace SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.RenewSubscriptionKeys
{
    public class RenewSubscriptionKeysCommand : IRequest<Unit>
    {
        public string InternalUserId { get; set; }
    }
}