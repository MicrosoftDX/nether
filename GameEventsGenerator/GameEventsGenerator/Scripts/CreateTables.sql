-- QUESTIONS
-- Number of players per day
-- Number of games per day
-- Time spent playing games per day
-- Which day / time of day do #CCUs peak?
-- What games does one player play the most?

-- SESSION DURATIONS
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'GameSessionDurations'))
BEGIN
CREATE TABLE [dbo].[GameSessionDurations] (
    [GameId] INT  NULL,
    [PlayerId]  NVARCHAR(max)   NULL,
    [EndTime] DATETIME2  NULL,
    [DurationInSeconds] INT NULL 
);
END

IF (NOT EXISTS (SELECT * FROM SYS.INDEXES 
                 WHERE NAME = 'IX_GameSessionDurations_EndTime'))
BEGIN
CREATE CLUSTERED INDEX [IX_GameSessionDurations_EndTime]
    ON [dbo].[GameSessionDurations]([EndTime] ASC);
END


-- GEO LOCATIONS
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'GeoLocations'))
BEGIN
CREATE TABLE [dbo].[GeoLocations] (
    [PlayerId]  NVARCHAR(450)	UNIQUE	NOT NULL,
    [Latitude]  VARCHAR(30) NULL,
    [Longitude] VARCHAR(30) NULL,
    [Country]   VARCHAR(30) NULL,
    [City]   NVARCHAR(max) NULL
);
END

IF (NOT EXISTS (SELECT * FROM SYS.INDEXES 
                 WHERE NAME = 'IX_GeoLocations'))
BEGIN
CREATE CLUSTERED INDEX [IX_GeoLocations]
    ON [dbo].[GeoLocations]([PlayerId] ASC);
END



-- NUMBER OF ACTIVE SESSIONS
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'ConcurrentUsers'))
BEGIN
CREATE TABLE [dbo].[ConcurrentUsers] (
    [GameId]  INT   NULL,
    [EndTime] DATETIME2  NULL,
    [NumberPlayers]  INT   NULL
);
END

IF (NOT EXISTS (SELECT * FROM SYS.INDEXES 
                 WHERE NAME = 'IX_ConcurrentUsers'))
BEGIN
CREATE CLUSTERED INDEX [IX_ConcurrentUsers]
    ON [dbo].[ConcurrentUsers]([EndTime] ASC);
END