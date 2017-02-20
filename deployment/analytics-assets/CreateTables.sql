-- Daily active users
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyActiveUsers'))
BEGIN
CREATE TABLE [dbo].[DailyActiveUsers] (
    [EventDate] DATE NOT NULL,
    [ActiveUsers] INT NOT NULL 
);
END


-- Monthly active users
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyActiveUsers'))
BEGIN
CREATE TABLE [dbo].[MonthlyActiveUsers] (
    [EventMonth] DATE NOT NULL,
    [ActiveUsers] INT NOT NULL 
);
END


-- Yearly active users
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'YearlyActiveUsers'))
BEGIN
CREATE TABLE [dbo].[YearlyActiveUsers] (
    [Year] INT NOT NULL,
    [ActiveUsers] INT NOT NULL 
);
END


-- Daily active sessions
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyActiveSessions'))
BEGIN
CREATE TABLE [dbo].[DailyActiveSessions] (
    [EventDate] DATE NOT NULL,
    [ActiveSessions] INT NOT NULL 
);
END


-- Monthly active sessions
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyActiveSessions'))
BEGIN
CREATE TABLE [dbo].[MonthlyActiveSessions] (
    [EventMonth] DATE NOT NULL,
    [ActiveSessions] INT NOT NULL 
);
END


-- Yearly active sessions
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'YearlyActiveSessions'))
BEGIN
CREATE TABLE [dbo].[YearlyActiveSessions] (
    [Year] INT NOT NULL,
    [ActiveSessions] INT NOT NULL 
);
END


-- Durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'Durations'))
BEGIN
CREATE TABLE [dbo].[Durations] (
    [StartTime] DATETIME NOT NOT NULL,
    [StopTime]  DATETIME NOT NOT NULL,
    [Duration] BIGINT NOT NOT NULL,
    [EventCorrelationId] TEXT NOT NOT NULL,
    [DisplayName] TEXT NOT NOT NULL,
    [GameSessionId] TEXT NOT NOT NULL
);
END


-- Daily Durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyDurations'))
BEGIN
CREATE TABLE [dbo].[DailyDurations] (
    [EventDate] DATE NOT NULL,
    [DisplayName] TEXT NOT NOT NULL,
    [AverageGenericDuration] BIGINT NOT NULL 
);
END


-- Monthly Durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyDurations'))
BEGIN
CREATE TABLE [dbo].[MonthlyDurations] (
    [EventMonth] DATE NOT NULL,
    [DisplayName] TEXT NOT NOT NULL,
    [AverageGenericDuration] BIGINT NOT NULL 
);
END


-- Annual Durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'YearlyDurations'))
BEGIN
CREATE TABLE [dbo].[YearlyDurations] (
    [Year] INT NOT NULL,
    [DisplayName] TEXT NOT NOT NULL,
    [AverageGenericDuration] BIGINT NOT NULL 
);
END


-- Daily game durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyGameDurations'))
BEGIN
CREATE TABLE [dbo].[DailyGameDurations] (
    [EventDate] DATE NOT NULL,
    [AverageGameDuration] BIGINT NOT NULL 
);
END


-- Monthly game durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyGameDurations'))
BEGIN
CREATE TABLE [dbo].[MonthlyGameDurations] (
    [EventMonth] DATE NOT NULL,
    [AverageGameDuration] BIGINT NOT NULL 
);
END


-- Yearly game durations
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'YearlyGameDurations'))
BEGIN
CREATE TABLE [dbo].[YearlyGameDurations] (
    [Year] INT NOT NULL,
    [AverageGameDuration] BIGINT NOT NULL 
);
END


-- Daily Level Dropoff Distribution
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'DailyLevelDropoff'))
BEGIN
CREATE TABLE [dbo].[DailyLevelDropoff] (
    [EventDate] DATE NOT NULL,
    [ReachedLevel] BIGINT NOT NULL,
    [TotalCount] BIGINT NOT NULL 
);
END


-- Monthly Level Dropoff Distribution
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'MonthlyLevelDropoff'))
BEGIN
CREATE TABLE [dbo].[MonthlyLevelDropoff] (
    [EventMonth] DATE NOT NULL,
    [ReachedLevel] BIGINT NOT NULL,
    [TotalCount] BIGINT NOT NULL 
);
END


-- Yearly Level Dropoff Distribution
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'YearlyLevelDropoff'))
BEGIN
CREATE TABLE [dbo].[YearlyLevelDropoff] (
    [Year] INT NOT NULL,
    [ReachedLevel] BIGINT NOT NULL,
    [TotalCount] BIGINT NOT NULL 
);
END