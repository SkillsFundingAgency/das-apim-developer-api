using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Services;
using SFA.DAS.Apim.Developer.Domain.Entities;
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
            GetUserSubscriptionsResponse userSubscriptionResponse,
            GetUserSubscriptionSecretsResponse userSubscriptionSecretsResponse,
            GetUserSubscriptionSecretsResponse userSubscriptionSecretsResponseSandbox,
            GetProductItem productItemResponse,
            GetProductItem productItemResponseSandbox,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            SubscriptionService service)
        {
            //Arrange
            var userType = ApimUserType.Provider;
            userSubscriptionResponse.Value.First().Name = $"{userType}-{internalUserId}";
            userSubscriptionResponse.Value.First().Properties.Scope = $"{userSubscriptionResponse.Value.First().Properties.Scope}/{product}";
            userSubscriptionResponse.Value.Last().Name = $"{userType}-{internalUserId}-sandbox";
            userSubscriptionResponse.Value.Last().Properties.Scope = $"{userSubscriptionResponse.Value.Last().Properties.Scope}/{productSandbox}";
            
            azureApimManagementService
                .Setup(x => x.Get<GetUserSubscriptionsResponse>(
                    It.Is<GetUserSubscriptionsRequest>(c => c.GetUrl.Contains($"{userType}-{internalUserId}"))))
                .ReturnsAsync(new ApiResponse<GetUserSubscriptionsResponse>(userSubscriptionResponse, HttpStatusCode.OK, null));
            azureApimManagementService
                .Setup(c => c.Post<GetUserSubscriptionSecretsResponse>(
                    It.Is<GetUserSubscriptionSecretsRequest>(x =>
                        x.PostUrl.Contains(userSubscriptionResponse.Value.First().Name)))).ReturnsAsync(
                    new ApiResponse<GetUserSubscriptionSecretsResponse>(userSubscriptionSecretsResponse,
                        HttpStatusCode.OK, null));
            azureApimManagementService
                .Setup(c => c.Post<GetUserSubscriptionSecretsResponse>(
                    It.Is<GetUserSubscriptionSecretsRequest>(x =>
                        x.PostUrl.Contains(userSubscriptionResponse.Value.Last().Name)))).ReturnsAsync(
                    new ApiResponse<GetUserSubscriptionSecretsResponse>(userSubscriptionSecretsResponseSandbox,
                        HttpStatusCode.OK, null));
            azureApimManagementService
                .Setup(c => c.Get<GetProductItem>(It.Is<GetProductRequest>(x =>
                    x.GetUrl.Contains(product))))
                .ReturnsAsync(new ApiResponse<GetProductItem>(productItemResponse, HttpStatusCode.OK, null));
            azureApimManagementService
                .Setup(c => c.Get<GetProductItem>(It.Is<GetProductRequest>(x =>
                    x.GetUrl.Contains(productSandbox))))
                .ReturnsAsync(new ApiResponse<GetProductItem>(productItemResponseSandbox, HttpStatusCode.OK, null));
            
            //Act
            var actual = await service.GetUserSubscriptions(internalUserId, userType);
            
            //Assert
            actual.Should().BeEquivalentTo(new List<UserSubscription>
            {
                new UserSubscription
                {
                    Id = userSubscriptionResponse.Value.First().Id,
                    Name = productItemResponse.Name,
                    PrimaryKey = userSubscriptionSecretsResponse.PrimaryKey
                },
                new UserSubscription
                {
                    Id = userSubscriptionResponse.Value.Last().Id,
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
            var userSubscriptionResponse = new GetUserSubscriptionsResponse
            {
                Value = new List<UserSubscriptionItem>(),
                Count = 0
            };
            azureApimManagementService
                .Setup(x => x.Get<GetUserSubscriptionsResponse>(
                    It.Is<GetUserSubscriptionsRequest>(c => c.GetUrl.Contains($"{userType}-{internalUserId}"))))
                .ReturnsAsync(new ApiResponse<GetUserSubscriptionsResponse>(userSubscriptionResponse, HttpStatusCode.OK, null));
            
            //Act
            var actual = await service.GetUserSubscriptions(internalUserId, userType);
            
            //Assert
            actual.Should().BeEmpty();
        }
    }
}