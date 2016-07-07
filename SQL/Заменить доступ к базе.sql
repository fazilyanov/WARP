Use Archive;
DECLARE @sql AS NVARCHAR(MAX) =''
DECLARE @user_id AS NVARCHAR(MAX) ='5521'
DECLARE @role_id AS NVARCHAR(MAX) ='12'   --14 осн ред -- 8 супер 
DECLARE @base_id AS NVARCHAR(MAX) ='1' -- 1- stg 

SET @sql = 'DELETE FROM [dbo].[_user_role_base] WHERE id_base = ' + @base_id + ' AND id_user = ' + @user_id
EXEC sp_executesql 	@sql

SET @sql = 'INSERT INTO [dbo].[_user_role_base]([id_user],[id_role],[id_base],[del])VALUES('+@user_id+','+@role_id+',' + @base_id + ',0)'
EXEC sp_executesql 	@sql