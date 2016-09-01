-- SESSION DURATIONS
IF EXISTS ( SELECT * FROM SYS.INDEXES 
            WHERE NAME = 'IX_GameSessionDurations_EndTime')
DROP INDEX [IX_GameSessionDurations_EndTime] ON [dbo].[GameSessionDurations];

IF EXISTS ( SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME = 'GameSessionDurations')
DROP TABLE [dbo].[GameSessionDurations];


-- GEO LOCATIONS
IF EXISTS ( SELECT * FROM SYS.INDEXES 
            WHERE NAME = 'IX_GeoLocations')
DROP INDEX [IX_GeoLocations] ON [dbo].[GeoLocations];

IF EXISTS ( SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME = 'GeoLocations')
DROP TABLE [dbo].[GeoLocations];


-- NUMBER OF ACTIVE SESSIONS
IF EXISTS ( SELECT * FROM SYS.INDEXES 
            WHERE NAME = 'IX_ConcurrentUsers')
DROP INDEX [IX_ConcurrentUsers] ON [dbo].[ConcurrentUsers];

IF EXISTS ( SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME = 'ConcurrentUsers')
DROP TABLE [dbo].[ConcurrentUsers];