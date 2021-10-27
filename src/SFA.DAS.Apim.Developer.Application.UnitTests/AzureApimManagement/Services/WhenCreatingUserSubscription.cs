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
            ApiResponse<CreateSubscriptionResponse> createSandboxSubscriptionApiResponse,
            [Frozen] Mock<IAzureApimManagementService> mockAzureApimManagementService,
            [Frozen] Mock<IUserService> mockUserService,
            SubscriptionService subscriptionService)
        {
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}";
            var expectedSandboxSubscriptionId = $"{apimUserType}-{internalUserId}-sandbox";
            mockUserService.Setup(x => x.CreateUser(internalUserId,userDetails, apimUserType)).ReturnsAsync(apimUserId);
            mockAzureApimManagementService.Setup(x =>
                x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                    c.PutUrl.Contains($"subscriptions/{expectedSubscriptionId}?")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUserId}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSubscriptionId)
                ))).ReturnsAsync(createSubscriptionApiResponse);
            mockAzureApimManagementService.Setup(x =>
                x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                    c.PutUrl.Contains($"subscriptions/{expectedSandboxSubscriptionId}?")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUserId}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSandboxSubscriptionId)
                ))).ReturnsAsync(createSandboxSubscriptionApiResponse);
            
            var actual = await subscriptionService.CreateUserSubscription(internalUserId, apimUserType, productName, userDetails);

            actual.Id.Should().Be(createSubscriptionApiResponse.Body.Id);
            actual.Name.Should().Be(createSubscriptionApiResponse.Body.Name);
            actual.PrimaryKey.Should().Be(createSubscriptionApiResponse.Body.Properties.PrimaryKey);
            actual.SandboxPrimaryKey.Should().Be(createSandboxSubscriptionApiResponse.Body.Properties.PrimaryKey);
        }
    }
}