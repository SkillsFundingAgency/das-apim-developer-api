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
using SFA.DAS.Apim.Developer.Domain.Subscriptions.Api.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Services
{
    public class WhenRegenerateSubscriptionKeys
    {
        [Test, MoqAutoData]
        public async Task Then_The_Subscription_Keys_Are_Regenerated_And_Audited(
            string internalUserId,
            ApimUserType apimUserType,
            string productName,
            [Frozen] Mock<IApimSubscriptionAuditRepository> apimSubscriptionAuditRepository,
            [Frozen] Mock<IAzureApimManagementService> mockAzureApimManagementService,
            [Frozen] Mock<IUserService> mockUserService,
            SubscriptionService subscriptionService)
        {
            var regeneratePrimaryResponse = new ApiResponse<string>(
                null, 
                HttpStatusCode.NoContent, 
                null);
            var regenerateSecondaryResponse = new ApiResponse<string>(
                null, 
                HttpStatusCode.NoContent, 
                null);
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}-{productName}";
            mockAzureApimManagementService.Setup(x =>
                x.Post<string>(It.Is<RegeneratePrimaryKeyRequest>(c => 
                    c.PostUrl.Contains($"subscriptions/{expectedSubscriptionId}/regeneratePrimaryKey?")
                ))).ReturnsAsync(regeneratePrimaryResponse);
            mockAzureApimManagementService.Setup(x =>
                x.Post<string>(It.Is<RegenerateSecondaryKeyRequest>(c => 
                    c.PostUrl.Contains($"subscriptions/{expectedSubscriptionId}/regenerateSecondaryKey?")
                ))).ReturnsAsync(regenerateSecondaryResponse);

            await subscriptionService.RegenerateSubscriptionKeys(internalUserId, apimUserType, productName);

            mockAzureApimManagementService.Verify(service => service.Post<string>(
                It.Is<RegeneratePrimaryKeyRequest>(c =>
                    c.PostUrl.Contains($"subscriptions/{expectedSubscriptionId}/regeneratePrimaryKey?"))), Times.Once);
            mockAzureApimManagementService.Verify(service => service.Post<string>(
                It.Is<RegenerateSecondaryKeyRequest>(c =>
                    c.PostUrl.Contains($"subscriptions/{expectedSubscriptionId}/regenerateSecondaryKey?"))), Times.Once);
            apimSubscriptionAuditRepository.Verify(x=>x.Insert(
                It.Is<ApimSubscriptionAudit>(c=>
                    c.Action.Equals("regenerate subscription")
                    && c.ProductName.Equals(productName)
                    && c.UserId.Equals(internalUserId)
                    && c.ApimUserType == (short)apimUserType
                    )), Times.Once);
        }
        
        [Test, MoqAutoData]
        public void And_Error_From_Api_Then_Throws_AggregateException(
            string internalUserId,
            ApimUserType apimUserType,
            string productName,
            [Frozen] Mock<IAzureApimManagementService> mockAzureApimManagementService,
            [Frozen] Mock<IUserService> mockUserService,
            SubscriptionService subscriptionService)
        {
            var regeneratePrimaryResponse = new ApiResponse<string>(
                null, 
                HttpStatusCode.NotModified, 
                "nasty little error");
            var regenerateSecondaryResponse = new ApiResponse<string>(
                null, 
                HttpStatusCode.NoContent, 
                null);
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}-{productName}";
            mockAzureApimManagementService.Setup(x =>
                x.Post<string>(It.Is<RegeneratePrimaryKeyRequest>(c => 
                    c.PostUrl.Contains($"subscriptions/{expectedSubscriptionId}/regeneratePrimaryKey?")
                ))).ReturnsAsync(regeneratePrimaryResponse);
            mockAzureApimManagementService.Setup(x =>
                x.Post<string>(It.Is<RegenerateSecondaryKeyRequest>(c => 
                    c.PostUrl.Contains($"subscriptions/{expectedSubscriptionId}/regenerateSecondaryKey?")
                ))).ReturnsAsync(regeneratePrimaryResponse);
            
            Func<Task> act = async () => await subscriptionService.RegenerateSubscriptionKeys(internalUserId, apimUserType, productName);

            act.Should().ThrowAsync<AggregateException>().Result
                .WithInnerException<ApplicationException>()
                .WithMessage($"*subscriptions/{expectedSubscriptionId}/regeneratePrimaryKey?*");
        }
        
        [Test, MoqAutoData]
        public void And_Multiple_Error_From_Api_Then_Throws_AggregateException_With_All_Exceptions(
            string internalUserId,
            ApimUserType apimUserType,
            string productName,
            [Frozen] Mock<IAzureApimManagementService> mockAzureApimManagementService,
            [Frozen] Mock<IUserService> mockUserService,
            SubscriptionService subscriptionService)
        {
            var regeneratePrimaryResponse = new ApiResponse<string>(
                null, 
                HttpStatusCode.NotModified, 
                "nasty little error");
            var regenerateSecondaryResponse = new ApiResponse<string>(
                null, 
                HttpStatusCode.NotModified, 
                "nasty little error");
            var expectedSubscriptionId = $"{apimUserType}-{internalUserId}-{productName}";
            mockAzureApimManagementService.Setup(x =>
                x.Post<string>(It.Is<RegeneratePrimaryKeyRequest>(c => 
                    c.PostUrl.Contains($"subscriptions/{expectedSubscriptionId}/regeneratePrimaryKey?")
                ))).ReturnsAsync(regeneratePrimaryResponse);
            mockAzureApimManagementService.Setup(x =>
                x.Post<string>(It.Is<RegenerateSecondaryKeyRequest>(c => 
                    c.PostUrl.Contains($"subscriptions/{expectedSubscriptionId}/regenerateSecondaryKey?")
                ))).ReturnsAsync(regenerateSecondaryResponse);
            
            Func<Task> act = async () => await subscriptionService.RegenerateSubscriptionKeys(internalUserId, apimUserType, productName);

            act.Should().ThrowAsync<AggregateException>().Result
                .Which.InnerExceptions.Count.Should().Be(2);
        }
    }
}