CREATE TABLE [apim_dev].[SubscriptionAudit]
(
    [SubscriptionId] INT NULL PRIMARY KEY FOREIGN KEY REFERENCES Subscription(Id),
    [UserRef] NVARCHAR(50) NOT NULL,
    [Action] NVARCHAR(50) NOT NULL
)