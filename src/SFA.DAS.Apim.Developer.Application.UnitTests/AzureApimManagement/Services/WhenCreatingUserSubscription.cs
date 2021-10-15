using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Entities;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api;
using SFA.DAS.Testing.AutoFixture;
using ApimUserType = SFA.DAS.Apim.Developer.Domain.Models.ApimUserType;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenCreatingUserSubscription
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_The_Subscription_Is_Created_For_The_User_And_Subscription_Returned(
            string internalUserId,
            ApimUserType apimUserType,
            string productName,
            string apimUserId,
            UserDetails userDetails,
            ApiResponse<CreateSubscriptionResponse> createSubscriptionApiResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IUserService> userService,
            SubscriptionService subscriptionService)
        {
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}";
            userService.Setup(x => x.CreateUser(internalUserId,userDetails, apimUserType)).ReturnsAsync(apimUserId);
            azureApimManagementService.Setup(x =>
                    x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                        c.PutUrl.Contains($"subscriptions/{expectedSubscriptionId}?")
                        && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                        && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUserId}")
                        && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSubscriptionId)
                    ))).ReturnsAsync(createSubscriptionApiResponse);
            
            var actual = await subscriptionService.CreateUserSubscription(internalUserId,apimUserType, productName, userDetails);

            actual.Should().Be(createSubscriptionApiResponse.Body);
        }
    }
}