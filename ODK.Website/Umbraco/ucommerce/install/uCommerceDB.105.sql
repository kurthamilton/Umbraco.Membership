IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'SortOrder' AND Object_ID = Object_ID(N'uCommerce_DataTypeEnum'))
BEGIN
	ALTER TABLE uCommerce_DataTypeEnum
	ADD SortOrder INT NOT NULL DEFAULT 0
END
GO
