CREATE TABLE [dbo].[Users]
(
	[UserId] INT NOT NULL PRIMARY KEY,
	[IsActive] BIT NOT NULL,
	[Role] NVARCHAR(50),
	[UserName] NVARCHAR(50),
	[PasswordHash] NVARCHAR(50),
	[FacebookUserId] NVARCHAR(100)
)

GO

CREATE UNIQUE INDEX [IX_Users_UserName] ON [dbo].[Users] ([UserName]) WHERE [UserName] IS NOT NULL
GO
CREATE UNIQUE INDEX [IX_Users_FacebookUserId]  ON [dbo].[Users] ([FacebookUserId]) WHERE [FacebookUserId] IS NOT NULL
GO
