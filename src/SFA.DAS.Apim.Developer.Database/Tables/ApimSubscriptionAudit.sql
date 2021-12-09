CREATE TABLE [dbo].[ApimSubscriptionAudit]
(
    [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY,
    [UserId] varchar(250) NOT NULL,
    [ProductName] VARCHAR(250) NOT NULL,
    [Action] VARCHAR(MAX) NOT NULL,
    [Timestamp] DATETIME NOT NULL
)
