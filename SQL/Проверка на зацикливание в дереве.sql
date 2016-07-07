use archive;

DECLARE @cur_parent AS nvarchar(50) = ''
	DECLARE @i AS int =0
	DECLARE @n_parents AS int =0

	IF OBJECT_ID(N'tempdb..#AllDoctree', N'U') IS NOT NULL drop table #AllDoctree;
CREATE TABLE #AllDoctree (ID nvarchar(50), name NVARCHAR(MAX), parent_ID nvarchar(50));

INSERT INTO #AllDoctree 
	SELECT id,
	org_name,
	'' as parent
	from
	[dbo].[_organization_1c]
	UNION ALL
	SELECT id, 
	[full_name],  
	CASE 
      WHEN [parent_ID] = '' THEN [organization_id]   
	  ELSE [parent_ID]
	END as parent 
	FROM [dbo].[_department_1c]


	IF OBJECT_ID(N'tempdb..#res', N'U') IS NOT NULL drop table #res;
	CREATE TABLE #res (id nvarchar(50),cnt int);

	IF OBJECT_ID(N'tempdb..#MainParents', N'U') IS NOT NULL drop table #MainParents;
	CREATE TABLE #MainParents (rnum int,id nvarchar(50));
	INSERT INTO #MainParents SELECT ROW_NUMBER()OVER(ORDER BY [name]), id FROM #AllDoctree ;
	--select * from  #MainParents;
	SET @n_parents=@@ROWCOUNT
	select @n_parents;
	SET @i = 1
	WHILE @i <= @n_parents
	BEGIN
		SELECT @cur_parent = id FROM #MainParents WHERE rnum = @i;
		insert into #res select @cur_parent as t, 0;
		--select  @i;
		--select  @cur_parent;
		WITH cteup AS(
			SELECT
				T.[id],
				T.[parent_ID],
				0 AS lvl
			FROM #AllDoctree T
			WHERE T.[id] = @cur_parent
			UNION ALL
			SELECT
				T.[id],
				T.[parent_ID],
				[cteup].[lvl] + 1 AS lvl
			FROM #AllDoctree T
				JOIN [cteup] ON T.[id] = [cteup].[parent_ID]
		)
		insert into #res select @cur_parent as t, Count([id])
		FROM cteup;
		
		SET @i=@i+1;
	END
	select * from #res;