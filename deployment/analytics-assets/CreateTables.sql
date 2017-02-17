-- Daily active users
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyActiveUsers'))
BEGIN
CREATE TABLE [dbo].[DailyActiveUsers] (
    [EventDate] DATE NULL,
    [ActiveUsers] INT NULL 
);
END


-- Monthly active users
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyActiveUsers'))
BEGIN
CREATE TABLE [dbo].[MonthlyActiveUsers] (
    [EventMonth] DATE NULL,
    [ActiveUsers] INT NULL 
);
END


-- Yearly active users
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'YearlyActiveUsers'))
BEGIN
CREATE TABLE [dbo].[YearlyDurations] (
    [Year] INT NULL,
    [ActiveUsers] INT NULL 
);
END


-- Daily active sessions
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyActiveSessions'))
BEGIN
CREATE TABLE [dbo].[DailyActiveSessions] (
    [EventDate] DATE NULL,
    [ActiveSessions] INT NULL 
);
END


-- Monthly active sessions
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyActiveSessions'))
BEGIN
CREATE TABLE [dbo].[MonthlyActiveSessions] (
    [EventMonth] DATE NULL,
    [ActiveSessions] INT NULL 
);
END


-- Yearly active sessions
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'YearlyActiveSessions'))
BEGIN
CREATE TABLE [dbo].[YearlyActiveSessions] (
    [Year] INT NULL,
    [ActiveSessions] INT NULL 
);
END


-- Durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'Durations'))
BEGIN
CREATE TABLE [dbo].[Durations] (
    [StartTime] DATETIME NOT NULL,
    [StopTime]  DATETIME NOT NULL,
    [Duration] BIGINT NOT NULL,
    [EventCorrelationId] TEXT NOT NULL,
    [DisplayName] TEXT NOT NULL,
    [GameSessionId] TEXT NOT NULL
);
END


-- Daily Durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyDurations'))
BEGIN
CREATE TABLE [dbo].[DailyDurations] (
    [EventDate] DATE NULL,
    [DisplayName] TEXT NOT NULL,
    [AverageGenericDuration] BIGINT NULL 
);
END


-- Monthly Durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyDurations'))
BEGIN
CREATE TABLE [dbo].[MonthlyDurations] (
    [EventMonth] DATE NULL,
    [DisplayName] TEXT NOT NULL,
    [AverageGenericDuration] BIGINT NULL 
);
END


-- Annual Durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'YearlyDurations'))
BEGIN
CREATE TABLE [dbo].[YearlyDurations] (
    [Year] INT NULL,
    [DisplayName] TEXT NOT NULL,
    [AverageGenericDuration] BIGINT NULL 
);
END


-- Daily game durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyGameDurations'))
BEGIN
CREATE TABLE [dbo].[DailyGameDurations] (
    [EventDate] DATE NULL,
    [AverageGameDuration] BIGINT NULL 
);
END


-- Monthly game durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyGameDurations'))
BEGIN
CREATE TABLE [dbo].[MonthlyGameDurations] (
    [EventDate] DATE NULL,
    [AverageGameDuration] BIGINT NULL 
);
END


-- Yearly game durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'YearlyGameDurations'))
BEGIN
CREATE TABLE [dbo].[YearlyGameDurations] (
    [Year] INT NULL,
    [AverageGameDuration] BIGINT NULL 
);
END