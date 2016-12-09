CREATE TABLE [dbo].[Groups]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(50) NOT NULL, 
    [CustomType] VARCHAR(50) NULL, 
    [Description] VARCHAR(50) NULL, 
    [Image] VARBINARY(50) NULL
)
