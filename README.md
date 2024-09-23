## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# APIM Developer API

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/APIM%20Developer/das-apim-developer-api?repoName=SkillsFundingAgency%2Fdas-apim-developer-api&branchName=main)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2604&repoName=SkillsFundingAgency%2Fdas-apim-developer-api&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-apim-developer-api&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-apim-developer-api)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

The APIM developer API is the inner API used to managed subscriptions to externally faced APIs. It also provides a mechanism for formatting the 
open API definition of the external API, excluding any headers that arent applicable.

## How It Works

The APIM Developer API connects to the Azure APIM API to allow the following operations.

* Get API Products
* Get Subscriptions
* Add Subscriptions
* Renew Subscriptions
* Delete Subscriptions
* Create User
* Update User
* Authenticate User

#### Get Products
The API products are retrieved by UserType, currently the available user types are 
* Documentation
* External
* Provider
* Employer

These types are defined against the API when registered in APIM. All APIs have the documentation type defined against them so that they appear in the [developer.apprenticeships.gov.uk](https://developer.apprenticeships.gov.uk) site

#### Get Subscriptions
The subscriptions are created in the following format `$"{apimUserType}-{internalUserId}-{productName}"` where the following is allowed:

**apimUserType** - Provider, Employer, External

**internalUserId** - this is the identifier, which is either the UKPRN, EmployerAccountId or External User Id GUID.

**productName** - The product which the user is subscribing to that is available to them

The subscriptions are then stored in Azure APIM

#### Create User
This is used to create a user that is able to access the API and is part of the **External** APIM user type group. 

#### Authenticate User
The authentication process is done using the Azure APIM api, this validates the credentials against what is stored in Azure APIM. No user information is stored in the database for this API

#### Auditing
There is an internal audit process that runs, this stores a record in the database to record when a subscription has been created or a subscription has been renewed. This is stored in a SQL database.


## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* A code editor that supports Azure functions and .NetCore 3.1
* An Azure Active Directory account with the appropriate roles as per the [config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-apim-developer-api)
* SQL server - Publish the `SFA.DAS.APIM.Developer.Database` project to create the SQL database
* Azure Storage Emulator(https://learn.microsoft.com/en-us/azure/storage/common/storage-use-emulator)

### Local running

The APIM developer api uses the standard Apprenticeship Service configuration. All configuration can be found in the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-apim-developer-api).

* appsettings.json file
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.Apim.Developer.Api",
  "Environment": "LOCAL",
  "Version": "1.0",
  "APPLICATIONINSIGHTS_CONNECTION_STRING": "",
  "AllowedHosts": "*"
}
```

You must have the Azure Storage emulator running, and in that a table created called `Configuration` in that table add the following:

Azure Table Storage config

Row Key: SFA.DAS.Apim.Developer.Api_1.0

Partition Key: LOCAL

Data:

```json
{
  "AzureApimManagement": {
    "ApimUserManagementUrl" : "https://{AZURE-APIM-URL}}",
    "ApimResourceId": "/subscriptions/{SUBSCRIPTION-ID}}/resourceGroups/{RESOURCE-GROUP-NAME}/providers/Microsoft.ApiManagement/service/{APIM-NAME}"
  },
  "ApimDeveloperApi": {
    "ConnectionString": "Data Source=.;Initial Catalog=SFA.DAS.Apim.Developer;Integrated Security=True;Pooling=False;Connect Timeout=30",
    "NumberOfAuthFailuresToLockAccount": 3,
    "AccountLockedDurationMinutes": 10
  },
  "AzureAd": {
    "Identifier": "https://{TENANT-NAME}/{IDENTIFIER}",
    "Tenant": "{TENANT-NAME}"
  }
}
```

## Technologies

* .NetCore 3.1
* Azure APIM API access with Azure APIM
* SQL
* Azure App Insights
* Azure Table Storage
* NUnit
* Moq
* FluentAssertions

## How It Works

### Running

* Open command prompt and change directory to _**/src/SFA.DAS.Apim.Developer.Api/**_
* Run the web project _**/src/SFA.DAS.Apim.Developer.Api.csproj**_

MacOS
```
ASPNETCORE_ENVIRONMENT=Development dotnet run
```
Windows cmd
```
set ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

### Application logs
Application logs are logged to [Application Insights](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview) and can be viewed using [Azure Monitor](https://learn.microsoft.com/en-us/azure/azure-monitor/overview) at https://portal.azure.com


## Useful URLs

### Products
https://localhost:5001/api/products - Endpoint to get API products are retrieved by UserType

### Subscription

https://localhost:5001/api/subscription/{id} - Endpoint to get user's subscription by Id

### User

https://localhost:5001/api/users - Endpoint to get all users

https://localhost:5001/api/users/authenticate - Endpoint to validate user credentials

https://localhost:5001/api/users/{id} - Endpoint to create/update user information

## 🐛 Known Issues

Do not run using IISExpress

## License

Licensed under the [MIT license](LICENSE)