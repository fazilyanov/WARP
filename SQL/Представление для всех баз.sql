USE [Archive]
GO

/****** Object:  View [dbo].[test_archive_depart_view]    Script Date: 06.02.2015 16:24:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'tempdb..#base', N'U') IS NOT NULL drop table #base;

SELECT 
    RowNum = ROW_NUMBER() OVER(ORDER BY id),*
INTO #base
FROM dbo._base;

DECLARE @MaxRownum INT
SET @MaxRownum = (SELECT MAX(RowNum) FROM #base)

DECLARE @Iter INT
SET @Iter = (SELECT MIN(RowNum) FROM #base)


DECLARE @base as varchar(MAX) = ''
DECLARE @sql as varchar(MAX) ;
WHILE @Iter <= @MaxRownum
BEGIN
	SET @base = (SELECT name FROM #base WHERE RowNum = @Iter);
	select @base;
	SET @sql='ALTER TABLE dbo.'+@base+'_docversion ADD id_quality tinyint NOT NULL CONSTRAINT DF_'+@base+'_docversion_id_quality DEFAULT 1;
	CREATE NONCLUSTERED INDEX IX_'+@base+'_docversion_id_quality ON dbo.'+@base+'_docversion	(	id_quality	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]';

	EXEC(@sql);

	SET @Iter = @Iter + 1
END









--DECLARE @base as varchar(MAX) = 'test'
--DECLARE @id_base as varchar(MAX) = ''

--DECLARE @base as varchar(MAX) = 'zao_stg' 
--DECLARE @id_base as varchar(MAX) = '1'

--DECLARE @base as varchar(MAX) = 'sib'
--DECLARE @id_base as varchar(MAX) = '3'

--DECLARE @base as varchar(MAX) = 'west' 
--DECLARE @id_base as varchar(MAX) = '7'

--DECLARE @base as varchar(MAX) = 'ngm'
--DECLARE @id_base as varchar(MAX) = '9'

--DECLARE @base as varchar(MAX) = 'north'
--DECLARE @id_base as varchar(MAX) = '6'

--DECLARE @base as varchar(MAX) = 'pvs' 
--DECLARE @id_base as varchar(MAX) = '11'

--DECLARE @base as varchar(MAX) = 'asm'
--DECLARE @id_base as varchar(MAX) = '2'

--DECLARE @base as varchar(MAX) = 'region'
--DECLARE @id_base as varchar(MAX) = '5'

--DECLARE @base as varchar(MAX) = 'autotrans'
--DECLARE @id_base as varchar(MAX) = '8'

--DECLARE @base as varchar(MAX) = 'complect'
--DECLARE @id_base as varchar(MAX) = '17'

--DECLARE @base as varchar(MAX) = 'logistic'
--DECLARE @id_base as varchar(MAX) = '16'

--DECLARE @base as varchar(MAX) = 'south'
--DECLARE @id_base as varchar(MAX) = '18'

--DECLARE @base as varchar(MAX) = 'diag'
--DECLARE @id_base as varchar(MAX) = '22'

--DECLARE @base as varchar(MAX) = 'energo'
--DECLARE @id_base as varchar(MAX) = '10'






