CREATE VIEW [DashboardReporting].[ApimSubscriptionAuditView]
AS
    SELECT 
        Id,
        UserId,
        ProductName,
        Action,
        TimeStamp
    FROM dbo.ApimSubscriptionAudit
GO