using System.Collections.Generic;


namespace SFA.DAS.Apim.Developer.Infrastructure.Models
{
    //https://docs.microsoft.com/en-us/rest/api/resources/resources/list
    public class AzureResourcesReponse
    {


        public class AzureResource
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public List<AzureResource> value { get; set; }
    }
}