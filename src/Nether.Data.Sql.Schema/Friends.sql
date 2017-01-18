/* Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license. See LICENSE file in the project root for full license information. */

CREATE TABLE [dbo].[Friends]
(
	[GamerTag] NVARCHAR(50) NOT NULL , 
    [FriendTag] NVARCHAR(50) NOT NULL, 
    PRIMARY KEY ([GamerTag], [FriendTag])
)
