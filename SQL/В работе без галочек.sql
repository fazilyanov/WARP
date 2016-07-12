/****** Script for SelectTopNRows command from SSMS  ******/
Select distinct id from (SELECT a.[id], a.[id_state],ISNULL(b.id,-1) as ttt
      
  FROM [Archive].[dbo].[zao_stg_archive] a
  LEFT JOIN [Archive].[dbo]._checkbox_list b on b.id_archive=a.id 
  and (b.name='checkbox1_zao_stg_archive_id_parent' or b.name='checkbox1_zao_stg_archive_id_prjcode' or b.name = 'checkbox1_zao_stg_archive_id_perf')
  where del=0 and id_state=5 and a.id_parent>0 and a.id_prjcode>0 and a.id_perf>0) s where s.ttt=-1 order by id