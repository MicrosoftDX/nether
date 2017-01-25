/* Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license. See LICENSE file in the project root for full license information. */

CREATE TABLE [dbo].[PlayerGroups]
(
    [Gamertag] varchar(50) NOT NULL, 
    [GroupName] varchar(50) NOT NULL, 
    CONSTRAINT [FK_PlayerGroups_Players] FOREIGN KEY ([Gamertag]) REFERENCES [Players](Gamertag), 
    CONSTRAINT [FK_PlayerGroups_Groups] FOREIGN KEY ([GroupName]) REFERENCES [Groups]([Name]), 
    CONSTRAINT [PK_PlayerGroups] PRIMARY KEY ([Gamertag], [GroupName])
)
GO

CREATE INDEX [IX_PlayerGroups_Gamertag] ON [dbo].[PlayerGroups] ([Gamertag])

GO

CREATE INDEX [IX_PlayerGroups_GroupName] ON [dbo].[PlayerGroups] ([GroupName])
