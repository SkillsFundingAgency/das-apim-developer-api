using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Products.Api.Responses;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenGettingUserSubscriptions
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_The_Subscription_Data_Is_Returned_From_The_Api_For_That_Account(
            string internalUserId,
            string product,
            string productSandbox,
            GetSubscriptionsResponse subscriptionResponse,
            GetUserSubscriptionSecretsResponse userSubscriptionSecretsResponse,
            GetUserSubscriptionSecretsResponse userSubscriptionSecretsResponseSandbox,
            GetProductItem productItemResponse,
            GetProductItem productItemResponseSandbox,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            SubscriptionService service)
        {
            //Arrange
            var userType = ApimUserType.Provider;
            subscriptionResponse.Value.First().Name = $"{userType}-{internalUserId}";
            subscriptionResponse.Value.First().Properties.Scope = $"{subscriptionResponse.Value.First().Properties.Scope}/{product}";
            subscriptionResponse.Value.Last().Name = $"{userType}-{internalUserId}-sandbox";
            subscriptionResponse.Value.Last().Properties.Scope = $"{subscriptionResponse.Value.Last().Properties.Scope}/{productSandbox}";
            
            azureApimManagementService
                .Setup(x => x.Get<GetSubscriptionsResponse>(
                    It.Is<GetUserSubscriptionsRequest>(c => c.GetUrl.Contains($"{userType}-{internalUserId}")), "application/json"))
                .ReturnsAsync(new ApiResponse<GetSubscriptionsResponse>(subscriptionResponse, HttpStatusCode.OK, null));
            azureApimManagementService
                .Setup(c => c.Post<GetUserSubscriptionSecretsResponse>(
                    It.Is<GetUserSubscriptionSecretsRequest>(x =>
                        x.PostUrl.Contains(subscriptionResponse.Value.First().Name)))).ReturnsAsync(
                    new ApiResponse<GetUserSubscriptionSecretsResponse>(userSubscriptionSecretsResponse,
                        HttpStatusCode.OK, null));
            azureApimManagementService
                .Setup(c => c.Post<GetUserSubscriptionSecretsResponse>(
                    It.Is<GetUserSubscriptionSecretsRequest>(x =>
                        x.PostUrl.Contains(subscriptionResponse.Value.Last().Name)))).ReturnsAsync(
                    new ApiResponse<GetUserSubscriptionSecretsResponse>(userSubscriptionSecretsResponseSandbox,
                        HttpStatusCode.OK, null));
            azureApimManagementService
                .Setup(c => c.Get<GetProductItem>(It.Is<GetProductRequest>(x =>
                    x.GetUrl.Contains(product)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetProductItem>(productItemResponse, HttpStatusCode.OK, null));
            azureApimManagementService
                .Setup(c => c.Get<GetProductItem>(It.Is<GetProductRequest>(x =>
                    x.GetUrl.Contains(productSandbox)), "application/json"))
                .ReturnsAsync(new ApiResponse<GetProductItem>(productItemResponseSandbox, HttpStatusCode.OK, null));
            
            //Act
            var actual = await service.GetUserSubscriptions(internalUserId, userType);
            
            //Assert
            actual.Should().BeEquivalentTo(new List<Subscription>
            {
                new Subscription
                {
                    Id = subscriptionResponse.Value.First().Id,
                    Name = productItemResponse.Name,
                    PrimaryKey = userSubscriptionSecretsResponse.PrimaryKey
                },
                new Subscription
                {
                    Id = subscriptionResponse.Value.Last().Id,
                    Name = productItemResponseSandbox.Name,
                    PrimaryKey = userSubscriptionSecretsResponseSandbox.PrimaryKey
                }
            });
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_If_No_Subscriptions_Empty_List_Returned(
            string internalUserId,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            SubscriptionService service)
        {
            //Arrange
            var userType = ApimUserType.Provider;
            var userSubscriptionResponse = new GetSubscriptionsResponse
            {
                Value = new List<SubscriptionItem>(),
                Count = 0
            };
            azureApimManagementService
                .Setup(x => x.Get<GetSubscriptionsResponse>(
                    It.Is<GetUserSubscriptionsRequest>(c => c.GetUrl.Contains($"{userType}-{internalUserId}")), "application/json"))
                .ReturnsAsync(new ApiResponse<GetSubscriptionsResponse>(userSubscriptionResponse, HttpStatusCode.OK, null));
            
            //Act
            var actual = await service.GetUserSubscriptions(internalUserId, userType);
            
            //Assert
            actual.Should().BeEmpty();
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_If_Request_Causes_A_Bad_Request_Then_Empty_List_Returned(
            string internalUserId,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            SubscriptionService service)
        {
            //Arrange
            var userType = ApimUserType.Provider;
            azureApimManagementService
                .Setup(x => x.Get<GetSubscriptionsResponse>(
                    It.Is<GetUserSubscriptionsRequest>(c => c.GetUrl.Contains($"{userType}-{internalUserId}")), "application/json"))
                .ReturnsAsync(new ApiResponse<GetSubscriptionsResponse>(null, HttpStatusCode.BadRequest, "Error"));
            
            //Act
            var actual = await service.GetUserSubscriptions(internalUserId, userType);
            
            //Assert
            actual.Should().BeEmpty();
        }
    }
}