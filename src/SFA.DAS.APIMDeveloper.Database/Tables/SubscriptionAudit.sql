CREATE TABLE [apim_dev].[SubscriptionAudit]
(
    [SubscriptionId] INT NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES [apim_dev].[Subscription](Id),
    [UserRef] NVARCHAR(50) NOT NULL,
    [Action] NVARCHAR(50) NOT NULL
)