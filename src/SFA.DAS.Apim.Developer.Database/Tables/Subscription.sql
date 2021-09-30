CREATE TABLE [dbo].[Subscription]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [ExternalSubscriptionId] INT NOT NULL,
    [ExternalSubscriberId] VARCHAR(50) NOT NULL,
    [SubscriberTypeId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES SubscriberType(Id)
)
