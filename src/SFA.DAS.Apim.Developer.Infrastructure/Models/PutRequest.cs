using System.Collections.Generic;


namespace SFA.DAS.Apim.Developer.Infrastructure.Models
{
    public class PutRequest
    {
        public string Url { get; set; }
        public AzureRequestBodyBase Body { get; set; }
    }
}