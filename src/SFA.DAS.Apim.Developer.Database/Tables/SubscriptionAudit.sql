CREATE TABLE [dbo].[SubscriptionAudit]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [SubscriptionId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Subscription(Id),
    [UserRef] VARCHAR(50) NOT NULL,
    [Action] VARCHAR(50) NOT NULL,
    [Timestamp] DATETIME NOT NULL
)