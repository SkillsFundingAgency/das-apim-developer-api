using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Api.ApiResponses;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Queries.GetProducts;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Api.UnitTests.ApiResponses
{
    public class WhenMappingFromMediatorResponseToGetProductsApiResponse
    {
        [Test, AutoData]
        public void Then_The_Fields_Are_Mapped(GetProductsQueryResponse source, JObject documentation)
        {
            foreach (var sourceProduct in source.Products)
            {
                sourceProduct.Documentation = JsonConvert.SerializeObject(documentation);
            }
            var actual = (GetProductsApiResponse)source;
            
            actual.Products.Should().BeEquivalentTo(source.Products, options => 
                options.Excluding(product => product.Documentation));
        }
        
        [Test, AutoData]
        public void Then_Does_Not_Add_Sandbox_To_DisplayName_If_In_Id(GetProductsQueryResponse source, JObject documentation)
        {
            foreach (var product in source.Products)
            {
                product.Id = product + "-Sandbox";
                product.Documentation = JsonConvert.SerializeObject(documentation);
            }
            var actual = (GetProductsApiResponse)source;
            
            actual.Products.Should().BeEquivalentTo(source.Products, options => 
                options.Excluding(product => product.DisplayName));
        }
        
        [Test, AutoData]
        public void Then_The_Non_Required_Security_Query_Options_Is_Removed_From_The_Documentation(Product product)
        {
            product.Documentation = testDocumentation;
            var source = new GetProductsQueryResponse
            {
                Products = new List<Product> { product }
            };
            
            var actual = (GetProductsApiResponse)source;

            var actualObject = JObject.Parse(actual.Products.First().Documentation);
            Assert.IsNotNull(actualObject);
            actualObject["security"]?.Count(c => c["apiKeyQuery"] != null).Should().Be(0);
        }
        
        [Test, AutoData]
        public void Then_The_Non_Required_Headers_Are_Removed_From_The_Documentation(Product product)
        {
            product.Documentation = testDocumentation;
            var source = new GetProductsQueryResponse
            {
                Products = new List<Product> { product }
            };
            
            var actual = (GetProductsApiResponse)source;

            var actualObject = JObject.Parse(actual.Products.First().Documentation);
            Assert.IsNotNull(actualObject);
            actualObject["paths"].First().First().First().Children()["parameters"].Values()
                .Any(c => c["name"].Value<string>() == "x-request-context-subscription-name").Should().BeFalse();
            actualObject["paths"].First().First().First().Children()["parameters"].Values()
                .Any(c => c["name"].Value<string>() == "x-request-context-subscription-is-sandbox").Should().BeFalse();
        }
        
        [Test, AutoData]
        public void Then_The_Version_Header_Is_Added_To_The_Documentation_And_Version_Set_As_Last_Char_Of_Product(Product product)
        {
            product.Documentation = testDocumentation;
            product.Name = "Some-Awesome-Api-Name-v8";
            var source = new GetProductsQueryResponse
            {
                Products = new List<Product> { product }
            };
            
            var actual = (GetProductsApiResponse)source;

            var actualObject = JObject.Parse(actual.Products.First().Documentation);
            Assert.IsNotNull(actualObject);
            actualObject["paths"].First().First().First().Children()["parameters"].Values()
                .Any(c => c["name"].Value<string>() == "X-Version").Should().BeTrue();
            actualObject["paths"].Last().Last().Last().Children()["parameters"].Values()
                .Any(c => c["name"].Value<string>() == "X-Version").Should().BeTrue();
            actualObject["paths"].First().First().First().Children()["parameters"].Values()["example"].Values<string>()
                .First().Should().Be("8");
        }

        [Test, AutoData]
        public void Then_The_SecurityScheme_For_Query_Is_Removed(Product product)
        {
            product.Documentation = testDocumentation;
            var source = new GetProductsQueryResponse
            {
                Products = new List<Product> { product }
            };
            
            var actual = (GetProductsApiResponse)source;

            var actualObject = JObject.Parse(actual.Products.First().Documentation);
            Assert.IsNotNull(actualObject);
            actualObject["components"]["securitySchemes"].Children().Values()
                .Any(c => c["name"].Value<string>() == "subscription-key").Should().BeFalse();
        }

        [Test, AutoData]
        public void Then_The_Servers_Url_Section_Is_Updated_And_Secure_EndPoint_Removed(Product product)
        {
            product.Documentation = testDocumentation;
            
            var source = new GetProductsQueryResponse
            {
                Products = new List<Product> { product }
            };
            
            var actual = (GetProductsApiResponse)source;

            var actualObject = JObject.Parse(actual.Products.First().Documentation);
            Assert.IsNotNull(actualObject);
            actualObject["servers"].First()["url"].Value<string>().Should().Be("https://something-api.apprenticeships.education.gov.uk/something");
        }
        
        [Test, AutoData]
        public void Then_The_Servers_Url_Section_Is_Updated_For_Sandbox(Product product)
        {
            testDocumentation = testDocumentation.Replace("https://something-gateway.apprenticeships.education.gov.uk/something", "https://something-gateway.apprenticeships.education.gov.uk/sandbox/something");
            product.Documentation = testDocumentation;
            product.Id = product + "-Sandbox";
            var source = new GetProductsQueryResponse
            {
                Products = new List<Product> { product }
            };
            
            var actual = (GetProductsApiResponse)source;

            var actualObject = JObject.Parse(actual.Products.First().Documentation);
            Assert.IsNotNull(actualObject);
            actualObject["servers"].First()["url"].Value<string>().Should().Be("https://something-api-sandbox.apprenticeships.education.gov.uk/something");
        }
        
        private string testDocumentation = @"{
                                                ""openapi"": ""3.0.1"",
                                                ""info"": {
                                                    ""title"": ""Recruitment API"",
                                                    ""description"": ""Create an advert on Find an apprenticeship using your existing systems."",
                                                    ""version"": ""1""
                                                },
                                                ""servers"": [
                                                    {
                                                        ""url"": ""https://something-gateway.apprenticeships.education.gov.uk/something""
                                                    },
                                                    {
                                                        ""url"": ""https://secure-something-gateway.apprenticeships.education.gov.uk/something""
                                                    }
                                                ],
                                                ""paths"": {
                                                    ""/accountlegalentities"": {
                                                        ""get"": {
                                                            ""tags"": [
                                                                ""AccountLegalEntities""
                                                            ],
                                                            ""summary"": ""/accountlegalentities - GET"",
                                                            ""operationId"": ""get-accountlegalentities"",
                                                            ""parameters"": [
                                                                {
                                                                    ""name"": ""x-request-context-subscription-name"",
                                                                    ""in"": ""header"",
                                                                    ""schema"": {
                                                                        ""type"": ""string""
                                                                    }
                                                                },
                                                                {
                                                                    ""name"": ""x-request-context-subscription-is-sandbox"",
                                                                    ""in"": ""header"",
                                                                    ""schema"": {
                                                                        ""type"": ""string""
                                                                    }
                                                                }
                                                            ],
                                                            ""responses"": {
                                                                ""200"": {
                                                                    ""description"": ""Success""
                                                                }
                                                            }
                                                        }
                                                    },
                                                    ""/referencedata/qualifications"": {
                                                        ""get"": {
                                                            ""tags"": [
                                                                ""ReferenceData""
                                                            ],
                                                            ""summary"": ""/referencedata/qualifications - GET"",
                                                            ""operationId"": ""get-referencedata-qualifications"",
                                                            ""responses"": {
                                                                ""200"": {
                                                                    ""description"": ""Success""
                                                                }
                                                            }
                                                        }
                                                    }
                                                },
                                                ""components"": {
                                                    ""schemas"": {},
                                                    ""securitySchemes"": {
                                                        ""apiKeyHeader"": {
                                                            ""type"": ""apiKey"",
                                                            ""name"": ""Ocp-Apim-Subscription-Key"",
                                                            ""in"": ""header""
                                                        },
                                                        ""apiKeyQuery"": {
                                                            ""type"": ""apiKey"",
                                                            ""name"": ""subscription-key"",
                                                            ""in"": ""query""
                                                        }
                                                    }
                                                },
                                                ""security"": [
                                                    {
                                                        ""apiKeyHeader"": []
                                                    },
                                                    {
                                                        ""apiKeyQuery"": []
                                                    }
                                                ]
                                            }";
    }
}
