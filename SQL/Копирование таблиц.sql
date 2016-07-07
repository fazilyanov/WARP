-----------------------------------------------------------------------------
---  Очищает таблицу, отключает автоинкремент, заливает, включает обратно
------------------------------------------------------------------------------

---archive_depart-------------------------------------------------------------
TRUNCATE TABLE [Archive].[dbo].[zao_stg_archive_depart]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_archive_depart] ON 
GO
INSERT INTO [Archive].[dbo].[zao_stg_archive_depart] (id, id_archive, id_depart)
SELECT id, id_archive, id_departs FROM [testbase].[dbo].[zao_stg_a_departs]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_archive_depart] OFF 
------------------------------------------------------------------------------


---Archive--------------------------------------------------------------------
TRUNCATE TABLE [Archive].[dbo].[zao_stg_archive]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_archive] ON 
GO
INSERT INTO [Archive].[dbo].[zao_stg_archive] 
([id],[doc_num],[date_doc],[date_upd],[prim],[content],[id_frm_contr],[id_frm_prod],[id_frm_dev],[id_docname],[id_doctype],[id_user_oper],[summ],[docpack],[id_structur_recipient],[id_structur_depart],[id_person_perf],[doctext],[id_prjcode], [hidden],[id_parent],[id_status])
SELECT 
 [id],[name]   ,[date_doc],[date_upd],[prim],[content],[id_frm2],     [id_frm1],    [id_frm3],   [id_docname],[id_doctype],[id_user],     [summ],[docpack],[id_structur1],         [id_structur2],      [id_perf],       [doctext],[id_prj_code],[hiden], [id_parent],[id_137]
FROM [testbase].[dbo].[zao_stg_archive] WHERE id>500000
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_archive] OFF 
------------------------------------------------------------------------------


---aunit----------------------------------------------------------------------
TRUNCATE TABLE [Archive].[dbo].[zao_stg_aunit]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_aunit] ON 
GO
INSERT INTO [Archive].[dbo].[zao_stg_aunit] 
([id],[id_type],[id_item])
SELECT 
 [id],[id_type],[id_item]
FROM [testbase].[dbo].[zao_stg_aunit]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_aunit] OFF 
------------------------------------------------------------------------------



---depart----------------------------------------------------------------------
TRUNCATE TABLE [Archive].[dbo].[zao_stg_depart]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_depart] ON 
GO
INSERT INTO [Archive].[dbo].[zao_stg_depart] 
([id],[name],[prim])
SELECT 
 [id],[n_depart],[opis]
FROM [testbase].[dbo].[zao_stg_departs]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_depart] OFF 
------------------------------------------------------------------------------



---doctree----------------------------------------------------------------------
TRUNCATE TABLE [Archive].[dbo].[zao_stg_doctree]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_doctree] ON 
GO
INSERT INTO [Archive].[dbo].[zao_stg_doctree] 
([id],[name],[id_parent],[form])
SELECT 
 [id],[treetext],[id_parent],'' as [form]
FROM [testbase].[dbo].[zao_stg_doctree]
GO
INSERT INTO [Archive].[dbo].[zao_stg_doctree] 
([id],[name],[id_parent],[form])
SELECT 
 ([id]+1000) as [id], CAST([name] as nvarchar(250)) as [name],[id_doctree],[form]
FROM [testbase].[dbo].[zao_stg_docname]
GO
UPDATE [Archive].[dbo].[zao_stg_archive]
   SET [id_docname] = [id_docname] + 1000
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_doctree] OFF 
------------------------------------------------------------------------------



---docversion-----------------------------------------------------------------
TRUNCATE TABLE [archive].[dbo].[zao_stg_docversion]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_docversion] ON 
GO
INSERT INTO [archive].[dbo].[zao_stg_docversion] 
([id],[id_archive],[nn],[date_reg],[path],[main],[prim],[id_user],[date_upd],[barcode],[id_user_t],[file_size])
SELECT 
 [id],[id_archive],[nn],[date_reg],[path],[main],[prim],[id_user],[date_upd],[barcode],[id_user_t],[file_size]
FROM [testbase].[dbo].[zao_stg_docversion] WHERE id_archive>500000
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_docversion] OFF 
------------------------------------------------------------------------------



---frm-----------------------------------------------------------------
TRUNCATE TABLE [archive].[dbo].[zao_stg_frm]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_frm] ON 
GO
INSERT INTO [archive].[dbo].[zao_stg_frm] 
([id],[name],[name_full],[inn])
SELECT 
 [id],[name],[name_full],[inn]
FROM [testbase].[dbo].[zao_stg_frm]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_frm] OFF 
------------------------------------------------------------------------------



---person-----------------------------------------------------------------
TRUNCATE TABLE [archive].[dbo].[zao_stg_person]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_person] ON 
GO
INSERT INTO [archive].[dbo].[zao_stg_person] 
([id],[name],[name_full],[id_depart])
SELECT 
 [id],[short_name],[p_name],[id_dlvtype]
FROM [testbase].[dbo].[zao_stg_persons]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_person] OFF 
------------------------------------------------------------------------------


---prjcode-----------------------------------------------------------------
TRUNCATE TABLE [archive].[dbo].[zao_stg_prjcode]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_prjcode] ON 
GO
INSERT INTO [archive].[dbo].[zao_stg_prjcode] 
([id],[code],[name],    [id_frm], [dogovor],[prim],[head])
SELECT 
 [id],[name],[prj_name],[id_frm2],[dogovor],[prim],[prj_head]
FROM [testbase].[dbo].[zao_stg_prj_code]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_prjcode] OFF 
------------------------------------------------------------------------------



---registry-----------------------------------------------------------------
TRUNCATE TABLE [archive].[dbo].[zao_stg_registry]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_registry] ON 
GO
INSERT INTO [archive].[dbo].[zao_stg_registry] 
([id],[date_upd],[id_user_oper],[date],[id_aunit_from],[id_aunit_to],[id_person_send],[id_person_recip],[prim],[id_operation],[id_archive])
SELECT 
 [id],[date_upd],[id_user]     ,[date],[id_sklad1]    ,[id_docplace],[id_sender]     ,[id_recipient]   ,[prim],[id_135]      ,[id_archive]
FROM [testbase].[dbo].[zao_stg_registry]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_registry] OFF 
------------------------------------------------------------------------------



---registry_detail-----------------------------------------------------------------
TRUNCATE TABLE [archive].[dbo].[zao_stg_registry_detail]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_registry_detail] ON 
GO
INSERT INTO [archive].[dbo].[zao_stg_registry_detail] 
([id],[id_registry],[copies],[prim],[id_docversion])
SELECT 
 [id],[id_registry],[copies],[prim],[id_docversion]
 FROM [testbase].[dbo].[zao_stg_registry_arc]
GO
SET IDENTITY_INSERT [archive].[dbo].[zao_stg_registry_detail] OFF 
------------------------------------------------------------------------------



---structur----------------------------------------------------------------------
TRUNCATE TABLE [Archive].[dbo].[zao_stg_structur]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_structur] ON 
GO
INSERT INTO [Archive].[dbo].[zao_stg_structur] 
([id],[name],[id_parent])
SELECT 
 [id],[name],[id_parent]
FROM [testbase].[dbo].[zao_stg_structur]
GO
SET IDENTITY_INSERT [Archive].[dbo].[zao_stg_structur] OFF 
------------------------------------------------------------------------------