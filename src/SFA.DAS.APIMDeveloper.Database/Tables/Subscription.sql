CREATE TABLE [apim_dev].[Subscription]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [ExternalSubscriptionId] INT NOT NULL,
    [ExternalSubscriberId] NVARCHAR(50) NOT NULL,
    [SubscriberTypeId] INT NOT NULL FOREIGN KEY REFERENCES SubscriberType(Id)
)