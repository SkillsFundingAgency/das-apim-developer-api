using System;
using System.Collections.Generic;
using System.IO;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Apim.Developer.Api.AppStart;
using SFA.DAS.Apim.Developer.Api.Infrastructure;
using SFA.DAS.Apim.Developer.Application.AzureApimManagement.Commands.CreateSubscription;
using SFA.DAS.Apim.Developer.Data;
using SFA.DAS.Apim.Developer.Domain.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Apim.Developer.Api
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IConfiguration configuration)
        {
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            if (!configuration!["Environment"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
#if DEBUG
                config
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile("appsettings.Development.json", true);
#endif

                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration!["ConfigNames"]!.Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["Environment"];
                        options.PreFixConfigurationKeys = false;
                    }
                );
            }

            _configuration = config.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var apimDeveloperApiConfiguration = _configuration
                .GetSection("ApimDeveloperApi")
                .Get<ApimDeveloperApiConfiguration>();

            if (!ConfigurationIsLocalOrDev())
            {
                var azureAdConfiguration = _configuration
                    .GetSection("AzureAd")
                    .Get<AzureActiveDirectoryConfiguration>();

                var policies = new Dictionary<string, string>
                {
                    {PolicyNames.Default, RoleNames.Default},
                };

                services.AddAuthentication(azureAdConfiguration, policies);
            }

            if (_configuration["Environment"] != "DEV")
            {
                services.AddHealthChecks()
                    .AddDbContextCheck<ApimDeveloperDataContext>();
            }

            services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(CreateSubscriptionCommand).Assembly));
            services.AddMediatorValidators();
            services.AddServiceRegistration();
            services.AddConfigurationOptions(_configuration);
            services.AddDatabaseRegistration(apimDeveloperApiConfiguration, _configuration["Environment"]);

            services
                .AddMvc(o =>
                {
                    if (!ConfigurationIsLocalOrDev())
                    {
                        o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                    }
                    o.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
                });

            services.AddOpenTelemetryRegistration(_configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]!);

			services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApimDeveloperApi", Version = "v1" });
                c.OperationFilter<SwaggerVersionHeaderFilter>();
                if (!ConfigurationIsLocalOrDev())
                {
                    c.OperationFilter<SwaggerAuthorizationHeaderFilter>();
                }
            });
            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            });
            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApimDeveloperApi");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            if (_configuration["Environment"] != "DEV")
            {
                app.UseHealthChecks();
            }

            app.UseRouting();
            app.UseEndpoints(builder =>
            {
                builder.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller=Subscriptions}/{action=Index}/{id?}");
            });
        }

        private bool ConfigurationIsLocalOrDev()
        {
            return _configuration["Environment"]!.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                   _configuration["Environment"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}