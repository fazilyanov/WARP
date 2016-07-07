SELECT execquery.last_execution_time AS [Date Time], execsql.text AS [Script] FROM sys.dm_exec_query_stats AS execquery
CROSS APPLY sys.dm_exec_sql_text(execquery.sql_handle) AS execsql
where  execquery.last_execution_time>'2015-04-17 09:27:00.000' and execquery.last_execution_time<'2015-04-17 09:28:00.000'
order by execquery.last_execution_time desc