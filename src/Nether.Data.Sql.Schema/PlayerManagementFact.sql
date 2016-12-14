CREATE TABLE [dbo].[PlayerManagementFact]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
    [PlayerId] UNIQUEIDENTIFIER NOT NULL, 
    [GroupId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_PlayerManagementFact_Players] FOREIGN KEY ([PlayerId]) REFERENCES [Players](Id), 
    CONSTRAINT [FK_PlayerManagementFact_Groups] FOREIGN KEY ([GroupId]) REFERENCES [Groups]([Id])
)
GO

CREATE INDEX [IX_PlayerManagementFact_PlayerId] ON [dbo].[PlayerManagementFact] ([PlayerId])

GO

CREATE INDEX [IX_PlayerManagementFact_GroupId] ON [dbo].[PlayerManagementFact] ([GroupId])
