USE master;  
declare @dbname sysname 
set @dbname = 'testbase'
select * from sysprocesses
where dbid = db_id(@dbname) and spid > 50