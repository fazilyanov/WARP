USE [Archive]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

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

--DECLARE @base as varchar(MAX) = 'tps'
--DECLARE @id_base as varchar(MAX) = '24'


-- '+@base+'
;DECLARE @sql as varchar(MAX) ;

set @sql='
TRUNCATE TABLE [dbo].['+@base+'_department]
;
SET IDENTITY_INSERT [dbo].['+@base+'_department] ON 
;
INSERT INTO [dbo].['+@base+'_department] (id, name) SELECT id, name FROM [dbo].['+@base+'_depart]
;
SET IDENTITY_INSERT [dbo].['+@base+'_depart] OFF 
'
EXEC(@sql);











--set @sql='ALTER TABLE dbo.'+@base+'_person ADD
--	id_depart int NOT NULL CONSTRAINT DF_'+@base+'_person_id_depart DEFAULT 0

--CREATE NONCLUSTERED INDEX IX_'+@base+'_person_id_depart ON dbo.'+@base+'_person
--	(
--	id
--	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--'
--EXEC(@sql);








--set @sql='
--	CREATE TABLE [dbo].['+@base+'_department](
--	[id] [int] IDENTITY(1,1) NOT NULL,
--	[name] [nvarchar](250) NOT NULL,
--	[id_1c] [nvarchar](50) NOT NULL CONSTRAINT [DF_'+@base+'_department_id_1c]  DEFAULT (''''),
--	[del] [bit] NOT NULL CONSTRAINT [DF_'+@base+'_department_del]  DEFAULT ((0)),
--	[id_parent] [int] NOT NULL CONSTRAINT [DF_'+@base+'_department_id_parent]  DEFAULT ((0)),
-- CONSTRAINT [PK_'+@base+'_department] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY];

--CREATE TABLE [dbo].['+@base+'_department_pre](
--	[id] [nvarchar](50) NOT NULL,
--	[treetext] [nvarchar](max) NOT NULL,
--	[tree_loaded] [bit] NOT NULL,
--	[tree_parent] [nvarchar](50) NOT NULL,
--	[tree_level] [int] NOT NULL,
--	[tree_leaf] [bit] NOT NULL,
--	[pos] [int] NOT NULL,
--	[top_parent] [nvarchar](50) NOT NULL,
--	[tree_expanded] [bit] NOT NULL,
-- CONSTRAINT [PK_'+@base+'_department_pre] PRIMARY KEY CLUSTERED 
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

--SET ANSI_PADDING ON;


--CREATE NONCLUSTERED INDEX [IX_'+@base+'_department_id_1c] ON [dbo].['+@base+'_department]
--(
--	[id_1c] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

--CREATE NONCLUSTERED INDEX [IX_'+@base+'_department_id_parent] ON [dbo].['+@base+'_department]
--(
--	[id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
--CREATE NONCLUSTERED INDEX [IX_'+@base+'_department_name] ON [dbo].['+@base+'_department]
--(
--	[name] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
--CREATE NONCLUSTERED INDEX [IX_'+@base+'_department_pre_pos] ON [dbo].['+@base+'_department_pre]
--(
--	[pos] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
--CREATE NONCLUSTERED INDEX [IX_'+@base+'_department_pre_top_parent] ON [dbo].['+@base+'_department_pre]
--(
--	[top_parent] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
--CREATE NONCLUSTERED INDEX [IX_'+@base+'_department_pre_tree_parent] ON [dbo].['+@base+'_department_pre]
--(
--	[tree_parent] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];'

----select @sql; 
--EXEC(@sql);

--set @sql='
--CREATE TRIGGER [dbo].['+@base+'_UpdateDepartmentPre]
--   ON  [dbo].['+@base+'_department] 
--   AFTER INSERT,DELETE,UPDATE
--AS 
--BEGIN
--	SET NOCOUNT ON;
--	EXEC [dbo].[PreDepartmentTree]  @from='''+@base+'_department'', @to='''+@base+'_department_pre''
--END;'
--EXEC(@sql);

