/* Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license. See LICENSE file in the project root for full license information. */

CREATE TABLE [dbo].[Scores]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [Score] INT NOT NULL, 
    [GamerTag] NVARCHAR(50) NOT NULL, 
    [CustomTag] NVARCHAR(50) NULL, 
    [DateAchieved] DATETIME NOT NULL DEFAULT GETUTCDATE() 
)

GO

CREATE INDEX [IX_Scores_1] ON [dbo].[Scores] ([DateAchieved], [GamerTag], [Score] DESC)
