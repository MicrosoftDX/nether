CREATE TABLE [dbo].[PlayerGroups]
(
    [Gamertag] varchar(50) NOT NULL, 
    [GroupName] varchar(50) NOT NULL, 
    CONSTRAINT [FK_PlayerGroups_Players] FOREIGN KEY ([Gamertag]) REFERENCES [Players](Gamertag), 
    CONSTRAINT [FK_PlayerGroups_Groups] FOREIGN KEY ([GroupName]) REFERENCES [Groups]([Name])
)
GO

CREATE INDEX [IX_PlayerGroups_Gamertag] ON [dbo].[PlayerGroups] ([Gamertag])

GO

CREATE INDEX [IX_PlayerGroups_GroupName] ON [dbo].[PlayerGroups] ([GroupName])
