CREATE VIEW [DashboardReporting].[ApimSubscriptionAuditView]
AS
    SELECT 
        Id,
        UserId,
        ProductName,
        Action,
        ApimUserType,
        TimeStamp
    FROM dbo.ApimSubscriptionAudit
GO