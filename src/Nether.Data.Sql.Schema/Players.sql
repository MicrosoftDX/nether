/* Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license. See LICENSE file in the project root for full license information. */

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

CREATE UNIQUE INDEX [IX_Players_Gamertag] ON [dbo].[Players] ([Gamertag])

GO

CREATE UNIQUE INDEX [IX_Players_UserId] ON [dbo].[Players] ([UserId])
