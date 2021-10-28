using System;
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
            CreateSubscriptionResponse createSubscriptionResponseBody,
            CreateSubscriptionResponse createSandboxSubscriptionResponseBody,
            [Frozen] Mock<IAzureApimManagementService> mockAzureApimManagementService,
            [Frozen] Mock<IUserService> mockUserService,
            SubscriptionService subscriptionService)
        {
            var createSubscriptionResponse = new ApiResponse<CreateSubscriptionResponse>(
                createSubscriptionResponseBody, 
                HttpStatusCode.OK, 
                null);
            var createSandboxSubscriptionResponse = new ApiResponse<CreateSubscriptionResponse>(
                createSandboxSubscriptionResponseBody, 
                HttpStatusCode.OK, 
                null);
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}";
            var expectedSandboxSubscriptionId = $"{apimUserType}-{internalUserId}-sandbox";
            mockUserService.Setup(x => x.CreateUser(internalUserId,userDetails, apimUserType)).ReturnsAsync(apimUserId);
            mockAzureApimManagementService.Setup(x =>
                x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                    c.PutUrl.Contains($"subscriptions/{expectedSubscriptionId}?")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUserId}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSubscriptionId)
                ))).ReturnsAsync(createSubscriptionResponse);
            mockAzureApimManagementService.Setup(x =>
                x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                    c.PutUrl.Contains($"subscriptions/{expectedSandboxSubscriptionId}?")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUserId}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSandboxSubscriptionId)
                ))).ReturnsAsync(createSandboxSubscriptionResponse);
            
            var actual = await subscriptionService.CreateUserSubscription(internalUserId, apimUserType, productName, userDetails);

            actual.Id.Should().Be(createSubscriptionResponseBody.Id);
            actual.Name.Should().Be(createSubscriptionResponseBody.Name);
            actual.PrimaryKey.Should().Be(createSubscriptionResponseBody.Properties.PrimaryKey);
            actual.SandboxPrimaryKey.Should().Be(createSandboxSubscriptionResponseBody.Properties.PrimaryKey);
        }
        
        [Test, RecursiveMoqAutoData]
        public void And_Error_Response_From_Subscription_Then_Throw_Exception(
            string internalUserId,
            ApimUserType apimUserType,
            string productName,
            string apimUserId,
            UserDetails userDetails,
            CreateSubscriptionResponse createSandboxSubscriptionResponseBody,
            [Frozen] Mock<IAzureApimManagementService> mockAzureApimManagementService,
            [Frozen] Mock<IUserService> mockUserService,
            SubscriptionService subscriptionService)
        {
            var createSubscriptionResponse = new ApiResponse<CreateSubscriptionResponse>(
                null, 
                HttpStatusCode.Forbidden, 
                "error message");
            var createSandboxSubscriptionResponse = new ApiResponse<CreateSubscriptionResponse>(
                createSandboxSubscriptionResponseBody, 
                HttpStatusCode.OK, 
                null);
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}";
            var expectedSandboxSubscriptionId = $"{apimUserType}-{internalUserId}-sandbox";
            mockUserService.Setup(x => x.CreateUser(internalUserId,userDetails, apimUserType)).ReturnsAsync(apimUserId);
            mockAzureApimManagementService.Setup(x =>
                x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                    c.PutUrl.Contains($"subscriptions/{expectedSubscriptionId}?")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUserId}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSubscriptionId)
                ))).ReturnsAsync(createSubscriptionResponse);
            mockAzureApimManagementService.Setup(x =>
                x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                    c.PutUrl.Contains($"subscriptions/{expectedSandboxSubscriptionId}?")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUserId}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSandboxSubscriptionId)
                ))).ReturnsAsync(createSandboxSubscriptionResponse);
            
            Func<Task> act = async () => await subscriptionService.CreateUserSubscription(internalUserId, apimUserType, productName, userDetails); 
            
            act.Should().Throw<InvalidOperationException>();
        }
        
        [Test, RecursiveMoqAutoData]
        public void And_Error_Response_From_Sandbox_Subscription_Then_Throw_Exception(
            string internalUserId,
            ApimUserType apimUserType,
            string productName,
            string apimUserId,
            UserDetails userDetails,
            CreateSubscriptionResponse createSubscriptionResponseBody,
            [Frozen] Mock<IAzureApimManagementService> mockAzureApimManagementService,
            [Frozen] Mock<IUserService> mockUserService,
            SubscriptionService subscriptionService)
        {
            var createSubscriptionResponse = new ApiResponse<CreateSubscriptionResponse>(
                createSubscriptionResponseBody, 
                HttpStatusCode.OK, 
                null);
            var createSandboxSubscriptionResponse = new ApiResponse<CreateSubscriptionResponse>(
                null, 
                HttpStatusCode.FailedDependency, 
                "error message");
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}";
            var expectedSandboxSubscriptionId = $"{apimUserType}-{internalUserId}-sandbox";
            mockUserService.Setup(x => x.CreateUser(internalUserId,userDetails, apimUserType)).ReturnsAsync(apimUserId);
            mockAzureApimManagementService.Setup(x =>
                x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                    c.PutUrl.Contains($"subscriptions/{expectedSubscriptionId}?")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUserId}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSubscriptionId)
                ))).ReturnsAsync(createSubscriptionResponse);
            mockAzureApimManagementService.Setup(x =>
                x.Put<CreateSubscriptionResponse>(It.Is<CreateSubscriptionRequest>(c => 
                    c.PutUrl.Contains($"subscriptions/{expectedSandboxSubscriptionId}?")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.Scope.Equals($"/products/{productName}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.OwnerId.Equals($"/users/{apimUserId}")
                    && ((CreateSubscriptionRequestBody)c.Data).Properties.DisplayName.Equals(expectedSandboxSubscriptionId)
                ))).ReturnsAsync(createSandboxSubscriptionResponse);
            
            Func<Task> act = async () => await subscriptionService.CreateUserSubscription(internalUserId, apimUserType, productName, userDetails); 
            
            act.Should().Throw<InvalidOperationException>();
        }
    }
}