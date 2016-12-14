CREATE TABLE [dbo].[PlayerGroups]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
    [PlayerId] UNIQUEIDENTIFIER NOT NULL, 
    [GroupId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_PlayerGroups_Players] FOREIGN KEY ([PlayerId]) REFERENCES [Players](Id), 
    CONSTRAINT [FK_PlayerGroups_Groups] FOREIGN KEY ([GroupId]) REFERENCES [Groups]([Id])
)
GO

CREATE INDEX [IX_PlayerGroups_PlayerId] ON [dbo].[PlayerGroups] ([PlayerId])

GO

CREATE INDEX [IX_PlayerGroups_GroupId] ON [dbo].[PlayerGroups] ([GroupId])
