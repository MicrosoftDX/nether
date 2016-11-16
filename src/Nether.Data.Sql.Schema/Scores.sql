CREATE TABLE [dbo].[Scores]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [Score] INT NOT NULL, 
    [GamerTag] NVARCHAR(50) NOT NULL, 
    [CutomTag] NVARCHAR(50) NULL, 
    [DateAchieved] DATETIME NOT NULL DEFAULT GETUTCDATE() 
)

GO

CREATE INDEX [IX_Scores_1] ON [dbo].[Scores] ([DateAchieved], [GamerTag], [Score] DESC)
