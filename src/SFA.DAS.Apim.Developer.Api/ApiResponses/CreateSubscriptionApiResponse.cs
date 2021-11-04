﻿using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateUserSubscription;

namespace SFA.DAS.Apim.Developer.Api.ApiResponses
{
    public class CreateSubscriptionApiResponse
    {
        public SubscriptionApiResponse LiveSubscription { get; set; }
        public SubscriptionApiResponse SandboxSubscription { get; set; }

        public static implicit operator CreateSubscriptionApiResponse(CreateUserSubscriptionCommandResponse source)
        {
            return new CreateSubscriptionApiResponse
            {
                LiveSubscription = new SubscriptionApiResponse{PrimaryKey = source.PrimaryKey},
                SandboxSubscription = new SubscriptionApiResponse{PrimaryKey = source.SandboxPrimaryKey}
            };
        }
    }
}