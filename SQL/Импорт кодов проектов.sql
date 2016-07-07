use Archive;
DECLARE @tb as varchar(100) = '';
DECLARE @ex as int = 0
DECLARE @fn as varchar(100) = ''
DECLARE @sql as varchar(MAX) = ''
DECLARE @i as int = 0
SET @tb = '_prjcode1'
set @i = 0


SET @fn = 'E:\backup\export\export.csv'
EXEC Master.dbo.xp_fileexist @fn, @ex OUT 
IF @ex = 1 
BEGIN
	SET @sql = 'TRUNCATE TABLE ' + @tb
	EXEC(@sql)
	SET @sql = 'BULK INSERT '+@tb +
	' FROM '''+ @fn +'''
	WITH
	(
		codepage=''1251'',
		FIRSTROW = 1,
		KEEPIDENTITY,
		FIELDTERMINATOR = '';'',
		ROWTERMINATOR = ''!*!\n''
	)'
	EXEC(@sql)
END
ELSE
BEGIN
	SELECT 'Õ≈“ ‘¿…À¿ '+@fn
END
