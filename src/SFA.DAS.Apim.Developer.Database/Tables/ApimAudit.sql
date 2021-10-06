CREATE TABLE [dbo].[ApimAudit]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
    [UserId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES ApimUser(ApimUserId),
    [Action] VARCHAR(50) NOT NULL,
    [Timestamp] DATETIME NOT NULL
)
