CREATE TABLE [dbo].[Friends]
(
	[GamerTag] NVARCHAR(50) NOT NULL , 
    [FriendTag] NVARCHAR(50) NOT NULL, 
    PRIMARY KEY ([GamerTag], [FriendTag])
)
