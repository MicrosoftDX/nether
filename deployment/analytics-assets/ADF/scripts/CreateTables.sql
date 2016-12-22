-- Daily active users
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyActiveUsers'))
BEGIN
CREATE TABLE [dbo].[DailyActiveUsers] (
    [Year] INT NULL,
    [Month]  INT NULL,
    [Day] INT NULL,
    [dau] INT NULL 
);
END


-- Monthly active users
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyActiveUsers'))
BEGIN
CREATE TABLE [dbo].[MonthlyActiveUsers] (
    [Year] INT NULL,
    [Month]  INT NULL,
    [mau] INT NULL 
);
END
