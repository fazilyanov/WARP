use Archive;
go

DECLARE	@return_value int

--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'zao_stg'
--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'asm'
--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'ooo_aps_east'

--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'ooo_ag'
--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'ooo_aps_north'
--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'ooo_aps_west'
--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'ooo_stg_autotrans'

--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'ooo_aps_ngm'
--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'ooo_asm'
--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'ooo_apvs'
--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'diag'
EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'tps'
--EXEC	@return_value = [dbo].[PreCatalog]	@p_base = N'logistic'

SELECT	'Return Value' = @return_value

GO