using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetUserSubscriptions;
using SFA.DAS.Apim.Developer.Domain.Interfaces;
using SFA.DAS.Apim.Developer.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Apim.Developer.Application.UnitTests.AzureApimManagement.Queries
{
    public class WhenHandlingGetUserSubscriptionsQuery
    {
        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_And_Data_Returned_For_Employer(
            GetUserSubscriptionsQuery query,
            List<Subscription> serviceResponse,
            [Frozen] Mock<ISubscriptionService> service,
            GetUserSubscriptionsQueryHandler handler)
        {
            service.Setup(x => x.GetUserSubscriptions(query.InternalUserId, ApimUserType.Employer)).ReturnsAsync(serviceResponse);

            var actual = await handler.Handle(query, CancellationToken.None);

            actual.UserSubscriptions.Should().BeEquivalentTo(serviceResponse);
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_And_Data_Returned_For_Provider(
            GetUserSubscriptionsQuery query,
            int providerId,
            List<Subscription> serviceResponse,
            [Frozen] Mock<ISubscriptionService> service,
            GetUserSubscriptionsQueryHandler handler)
        {
            query.InternalUserId = providerId.ToString();
            service.Setup(x => x.GetUserSubscriptions(query.InternalUserId, ApimUserType.Provider)).ReturnsAsync(serviceResponse);

            var actual = await handler.Handle(query, CancellationToken.None);

            actual.UserSubscriptions.Should().BeEquivalentTo(serviceResponse);
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_And_Data_Returned_For_External(
            GetUserSubscriptionsQuery query,
            Guid externalId,
            List<Subscription> serviceResponse,
            [Frozen] Mock<ISubscriptionService> service,
            GetUserSubscriptionsQueryHandler handler)
        {
            query.InternalUserId = externalId.ToString();
            service.Setup(x => x.GetUserSubscriptions(query.InternalUserId, ApimUserType.External)).ReturnsAsync(serviceResponse);

            var actual = await handler.Handle(query, CancellationToken.None);

            actual.UserSubscriptions.Should().BeEquivalentTo(serviceResponse);
        }
    }
}