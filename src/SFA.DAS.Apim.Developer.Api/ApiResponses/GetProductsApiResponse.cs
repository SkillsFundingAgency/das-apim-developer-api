using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetProducts;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Api.ApiResponses
{
    public class GetProductsApiResponse
    {
        public List<GetProductsApiResponseItem> Products { get; set; }

        public static implicit operator GetProductsApiResponse(GetProductsQueryResponse source)
        {
            return new GetProductsApiResponse
            {
                Products = source.Products.Select(c=>(GetProductsApiResponseItem)c).ToList()
            };
        }
    }

    public class GetProductsApiResponseItem
    {
        
        public string Id { get ; set ; }
        public string Name { get; set; }
        public string DisplayName { get ; set ; }
        public string Description { get ; set ; }
        public string Documentation { get ; set ; }
        public Dictionary<string,string> Documents { get; set; }


        public static implicit operator GetProductsApiResponseItem(Product source)
        {
            var isSandbox = source.Id.EndsWith("sandbox", StringComparison.InvariantCultureIgnoreCase);
            var documents = new Dictionary<string, string>();
            foreach (var sourceDocument in source.Documents)
            {
                var xVersionNumber = sourceDocument.Key.ToArray().Last().ToString();
                var documentationObject = PrepareOpenApiDocumentation(sourceDocument.Value, isSandbox, xVersionNumber);    
                documents.Add(sourceDocument.Key, documentationObject);
            }
            
            
            return new GetProductsApiResponseItem
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                DisplayName = source.DisplayName,
                Documentation = documents.Last().Value,
                Documents = documents
            };
        }

        private static string PrepareOpenApiDocumentation(string source, bool isSandbox, string xVersionNumber)
        {
            var notRequiredSecurityHeaders = new List<string>
                { "x-request-context-subscription-name", "x-request-context-subscription-is-sandbox" };
            var headerVersion = JObject.Parse(JsonConvert.SerializeObject(new HeaderVersion(xVersionNumber)));

            var documentationObject = JObject.Parse(source);
            documentationObject["security"]?.FirstOrDefault(c => c["apiKeyQuery"] != null)?.Remove();

            var secureUrlToRemove = documentationObject["servers"]?
                    .FirstOrDefault(x => x["url"].Value<string>().Contains("secure"))?["url"]?.Value<string>();

            if (!string.IsNullOrEmpty(secureUrlToRemove))
            {
                documentationObject["servers"]?.FirstOrDefault(c=>c["url"].Value<string>() == secureUrlToRemove)?.Remove();    
            }
            
            var url = documentationObject["servers"]?.FirstOrDefault()?["url"]?.Value<string>();

            if (!string.IsNullOrEmpty(url))
            {
                url = url.Replace(isSandbox ?"gateway.apprenticeships.education.gov.uk/sandbox":"gateway.apprenticeships.education.gov.uk",
                    isSandbox ? "api-sandbox.apprenticeships.education.gov.uk" : "api.apprenticeships.education.gov.uk");
                documentationObject["servers"]?.FirstOrDefault()?.AddAfterSelf(JObject.Parse(JsonConvert.SerializeObject(new {url})));
                documentationObject["servers"]?.FirstOrDefault()?.Remove();    
            }
            
            documentationObject["components"]?["securitySchemes"]?.Children().Values().FirstOrDefault(c => (c["name"] ?? "").Value<string>() == "subscription-key")?.Parent?.Remove();
            if (documentationObject["paths"] == null)
            {
                return JsonConvert.SerializeObject(documentationObject);
            }

            foreach (var firstPath in documentationObject["paths"])
            {
                foreach (var secondPath in firstPath)
                {
                    foreach (var thirdPath in secondPath)
                    {
                        var children = thirdPath.Children()["parameters"].Values();
                        if (children.Any())
                        {
                            children.FirstOrDefault()?.AddAfterSelf(headerVersion);
                            foreach (var notRequiredHeader in notRequiredSecurityHeaders)
                            {
                                children.FirstOrDefault(c =>
                                    c["name"].Value<string>()
                                        .Equals(notRequiredHeader, StringComparison.CurrentCultureIgnoreCase))?.Remove();
                            }
                        }
                        else
                        {
                            thirdPath.Children().Values().FirstOrDefault().AddAfterSelf(new JProperty("parameters",
                                JArray.Parse(JsonConvert.SerializeObject(new List<HeaderVersion> { new HeaderVersion(xVersionNumber) }))));
                            break;
                        }
                    }
                }
            }


            return JsonConvert.SerializeObject(documentationObject);
        }
        private class HeaderVersion
        {
            public HeaderVersion(string version)
            {
                Example = version;
            }
            [JsonProperty("name")]
            public string Name = "X-Version";
            [JsonProperty("in")]
            public string In = "header";
            [JsonProperty("schema")]
            public SchemaVersion Schema = new SchemaVersion();
            [JsonProperty("required")] 
            public bool Required = true;
            [JsonProperty("example")] 
            public string Example;
            public class SchemaVersion
            {
                [JsonProperty("type")]
                public string Type = "string";
            }
        }
    }
}