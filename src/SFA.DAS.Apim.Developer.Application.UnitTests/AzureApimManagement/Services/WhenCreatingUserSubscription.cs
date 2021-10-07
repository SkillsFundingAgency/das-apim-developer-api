using System;
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
        public async Task Then_If_The_User_Does_Not_Exist_It_Is_Created_In_The_Database_And_On_Service(
            string internalUserId, 
            ApimUserType apimUserType, 
            string productName, 
            Guid apimUserId,
            ApimUser apimUser,
            ApiResponse<CreateUserResponse> createUserApiResponse,
            ApiResponse<CreateSubscriptionResponse> createSubscriptionApiResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService, 
            [Frozen] Mock<IApimUserRepository> apimUserRepository, 
            SubscriptionService subscriptionService)
        {
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}";
            createUserApiResponse.Body.Name = apimUserId.ToString();
            apimUserRepository.Setup(x => x.GetByInternalIdAndType(internalUserId, (int) apimUserType)).ReturnsAsync((ApimUser)null);
            azureApimManagementService.Setup(x =>
                x.Put<CreateUserResponse>(It.Is<CreateUserRequest>(c => c.PutUrl.Contains($"users/"))))
                .ReturnsAsync(createUserApiResponse);
            apimUserRepository.Setup(x=>x.Insert(It.Is<ApimUser>(c=>
                c.ApimUserTypeId.Equals((int) apimUserType)
                && c.InternalUserId.Equals(internalUserId)
            ))).ReturnsAsync(apimUser);
            azureApimManagementService.Setup(x =>
                x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                    c.PutUrl.Contains($"subscriptions/{expectedSubscriptionId}?")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUser.ApimUserId}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSubscriptionId)
                ))).ReturnsAsync(createSubscriptionApiResponse);

            var actual = await subscriptionService.CreateUserSubscription(internalUserId,apimUserType, productName);
            
            actual.Should().Be(createSubscriptionApiResponse.Body.Properties.DisplayName);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_The_Subscription_Is_Created_For_The_User_And_Name_Returned(
            string internalUserId,
            ApimUserType apimUserType,
            string productName,
            ApimUser apimUser,
            ApiResponse<CreateSubscriptionResponse> createSubscriptionApiResponse,
            [Frozen] Mock<IAzureApimManagementService> azureApimManagementService,
            [Frozen] Mock<IApimUserRepository> apimUserRepository,
            SubscriptionService subscriptionService)
        {
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}";
            apimUserRepository.Setup(x => x.GetByInternalIdAndType(internalUserId, (int) apimUserType)).ReturnsAsync(apimUser);
            azureApimManagementService.Setup(x =>
                    x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                        c.PutUrl.Contains($"subscriptions/{expectedSubscriptionId}?")
                        && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                        && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUser.ApimUserId}")
                        && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSubscriptionId)
                    ))).ReturnsAsync(createSubscriptionApiResponse);
            
            var actual = await subscriptionService.CreateUserSubscription(internalUserId,apimUserType, productName);

            actual.Should().Be(createSubscriptionApiResponse.Body.Properties.DisplayName);
        }
    }
}