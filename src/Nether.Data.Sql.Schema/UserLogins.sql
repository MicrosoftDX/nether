CREATE TABLE [dbo].[UserLogins]
(
	[ProviderType] NVARCHAR(50) NOT NULL,
	[ProviderId] NVARCHAR(50) NOT NULL,
    [UserId] NVARCHAR(50) NOT NULL, 
    [ProviderData] NTEXT NULL, 
    PRIMARY KEY ([ProviderType], [ProviderId])
)

GO
