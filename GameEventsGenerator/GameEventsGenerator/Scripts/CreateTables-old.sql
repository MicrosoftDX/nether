-- TUMBLING WINDOW
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'GameEventsTumblingCount'))
BEGIN
CREATE TABLE [dbo].[GameEventsTumblingCount] (
    [GameId] INT  NULL,
    [WindowEnd]   DATETIME2  NULL,
    [Count]         INT     NULL
);
END

IF (NOT EXISTS (SELECT * FROM SYS.INDEXES 
                 WHERE NAME = 'IX_GameEventsTumblingCount_StartTime'))
BEGIN
CREATE CLUSTERED INDEX [IX_GameEventsTumblingCount_StartTime]
    ON [dbo].[GameEventsTumblingCount]([WindowEnd] ASC);
END


-- REFERENCE DATA JOIN EXAMPLE

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'GameEventsRefJoin'))
BEGIN
CREATE TABLE [dbo].[GameEventsRefJoin] (
    [EntryTime] DATETIME2  NULL,
    [PlayerId]   NVARCHAR(max)  NULL,
    [GameId] INT  NULL,
    [RegistrationId]   BIGINT  NULL
);
END

IF (NOT EXISTS (SELECT * FROM SYS.INDEXES 
                 WHERE NAME = 'IX_GameEventsRefJoin_EntryTime'))
BEGIN
CREATE CLUSTERED INDEX [IX_GameEventsRefJoin_EntryTime]
    ON [dbo].[GameEventsRefJoin]([EntryTime] ASC);
END

-- TUMBLING WINDOW PARTITIONED EXAMPLE
IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'GameEventsTumblingCountPartitioned'))
BEGIN
CREATE TABLE [dbo].[GameEventsTumblingCountPartitioned] (
    [GameId] INT  NULL,
    [WindowEnd]   DATETIME2  NULL,
    [Count]         INT     NULL
);
END

IF (NOT EXISTS (SELECT * FROM SYS.INDEXES 
                 WHERE NAME = 'IX_GameEventsTumblingCountPartitioned_StartTime'))
BEGIN
CREATE CLUSTERED INDEX [IX_GameEventsTumblingCountPartitioned_StartTime]
    ON [dbo].[GameEventsTumblingCountPartitioned]([WindowEnd] ASC);
END
