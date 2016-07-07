-- 2014-06-25
USE [testbase]
GO

exec sp_fulltext_catalog 'asm_catalog_doctext', 'start_full'
WAITFOR DELAY '00:00:15.000'
ALTER FULLTEXT INDEX ON [dbo].[asm_archive] SET CHANGE_TRACKING = AUTO
GO

exec sp_fulltext_catalog 'ooo_ag_catalog_doctext', 'start_full'
WAITFOR DELAY '00:00:15.000'
ALTER FULLTEXT INDEX ON [dbo].[ooo_ag_archive] SET CHANGE_TRACKING = AUTO
GO

exec sp_fulltext_catalog 'ooo_aps_east_catalog_doctext', 'start_full'
WAITFOR DELAY '00:00:15.000'
ALTER FULLTEXT INDEX ON [dbo].[ooo_aps_east_archive] SET CHANGE_TRACKING = AUTO
GO

exec sp_fulltext_catalog 'ooo_aps_ngm_catalog_doctext', 'start_full'
WAITFOR DELAY '00:00:15.000'
ALTER FULLTEXT INDEX ON [dbo].[ooo_aps_ngm_archive] SET CHANGE_TRACKING = AUTO
GO

exec sp_fulltext_catalog 'ooo_aps_north_catalog_doctext', 'start_full'
WAITFOR DELAY '00:00:15.000'
ALTER FULLTEXT INDEX ON [dbo].[ooo_aps_north_archive] SET CHANGE_TRACKING = AUTO
GO

exec sp_fulltext_catalog 'ooo_aps_west_catalog_doctext', 'start_full'
WAITFOR DELAY '00:00:15.000'
ALTER FULLTEXT INDEX ON [dbo].[ooo_aps_west_archive] SET CHANGE_TRACKING = AUTO
GO

exec sp_fulltext_catalog 'ooo_apvs_catalog_doctext', 'start_full'
WAITFOR DELAY '00:00:15.000'
ALTER FULLTEXT INDEX ON [dbo].[ooo_apvs_archive] SET CHANGE_TRACKING = AUTO
GO

exec sp_fulltext_catalog 'ooo_asm_catalog_doctext', 'start_full'
WAITFOR DELAY '00:00:15.000'
ALTER FULLTEXT INDEX ON [dbo].[ooo_asm_archive] SET CHANGE_TRACKING = AUTO
GO

exec sp_fulltext_catalog 'ooo_stg_autotrans_catalog_doctext', 'start_full'
WAITFOR DELAY '00:00:15.000'
ALTER FULLTEXT INDEX ON [dbo].[ooo_stg_autotrans_archive] SET CHANGE_TRACKING = AUTO
GO

--exec sp_fulltext_catalog 'zao_stg_catalog_doctext', 'start_full'
--WAITFOR DELAY '00:00:15.000'
--ALTER FULLTEXT INDEX ON [dbo].[zao_stg_archive] SET CHANGE_TRACKING = AUTO
--GO