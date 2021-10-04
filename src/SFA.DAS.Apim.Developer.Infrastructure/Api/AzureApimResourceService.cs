using System;
using System.Threading.Tasks;
using SFA.DAS.Apim.Developer.Domain.Interfaces;

namespace SFA.DAS.Apim.Developer.Infrastructure.Api
{
    public class AzureApimResourceService : IAzureApimResourceService
    {
        public Task<string> GetResourceId()
        {
            throw new NotImplementedException();
            //  var subsRequest = new GetRequest
            // {
            //     Url = $"{AzureResource}/subscriptions?api-version=2020-01-01"
            // };
            // var subs = await Get<AzureSubscriptionsResponse>(subsRequest);
            // if (subs.value.Count != 1)
            // {
            //     throw new System.Exception("Subscription count unexpected");
            // }
            // var subscriptionId = subs.value.First().subscriptionId;
            //
            // var apimRequest = new GetRequest
            // {
            //     Url = $"{AzureResource}/subscriptions/{subscriptionId}/resources?$filter=resourceType eq 'Microsoft.ApiManagement/service' and name eq '{_configuration.Value.ApimServiceName}'&api-version=2021-04-01"
            // };
            // var apimResources = await Get<AzureResourcesReponse>(apimRequest);
            // if (apimResources.value.Count != 1)
            // {
            //     throw new System.Exception("Apim Resources count unexpected");
            // }
            //
            // return apimResources.value.First().id;
        }
    }
}