CREATE TABLE [dbo].[Players]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
    [PlayerId] VARCHAR(50) NOT NULL , 
    [Gamertag] VARCHAR(50) NOT NULL, 
    [Country] VARCHAR(50) NULL, 
    [CustomTag] VARCHAR(50) NULL, 
    [PlayerImage] VARBINARY(50) NULL, 
    PRIMARY KEY ([Id]), 
    CONSTRAINT [AK_Players_PlayerId] UNIQUE ([PlayerId])
)


GO

CREATE INDEX [IX_Players_Gamertag] ON [dbo].[Players] ([Gamertag])

GO

CREATE INDEX [IX_Players_PlayerId] ON [dbo].[Players] ([PlayerId])
