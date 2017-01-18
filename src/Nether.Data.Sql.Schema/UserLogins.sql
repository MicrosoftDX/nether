/* Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license. See LICENSE file in the project root for full license information. */

CREATE TABLE [dbo].[UserLogins]
(
	[ProviderType] NVARCHAR(50) NOT NULL,
	[ProviderId] NVARCHAR(50) NOT NULL,
    [UserId] NVARCHAR(50) NOT NULL, 
    [ProviderData] NTEXT NULL, 
    PRIMARY KEY ([ProviderType], [ProviderId])
)

GO
