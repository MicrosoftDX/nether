/* Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license. See LICENSE file in the project root for full license information. */
CREATE TABLE [dbo].[Users]
(
	[UserId] NVARCHAR(50) NOT NULL PRIMARY KEY,
	[IsActive] BIT NOT NULL,
	[Role] NVARCHAR(50)
)

GO