CREATE TABLE [dbo].[Players]
(
    [Gamertag] VARCHAR(50) NOT NULL, 
    [Country] VARCHAR(50) NULL, 
    [CustomTag] VARCHAR(50) NULL, 
    [PlayerImage] VARBINARY(50) NULL, 
    [UserId] VARCHAR(50) NOT NULL , 
    PRIMARY KEY ([Gamertag]), 
    CONSTRAINT [AK_Players_PlayerId] UNIQUE ([Gamertag])
)


GO

CREATE INDEX [IX_Players_Gamertag] ON [dbo].[Players] ([Gamertag])

GO

CREATE INDEX [IX_Players_UserId] ON [dbo].[Players] ([UserId])
