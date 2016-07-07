USE [Archive]
GO
/****** Object:  Table [dbo].[zao_stg_a_departs]    Script Date: 18.06.2014 13:24:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_a_departs](
	[id] [int] NOT NULL,
	[id_archive] [int] NOT NULL,
	[id_departs] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_archive]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_archive](
	[id] [int] NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[date_doc] [date] NULL,
	[date_upd] [datetime] NULL,
	[prim] [nvarchar](250) NOT NULL,
	[content] [nvarchar](250) NOT NULL,
	[id_frm2] [int] NOT NULL,
	[id_docname] [int] NOT NULL,
	[id_doctype] [int] NOT NULL,
	[id_user] [int] NOT NULL,
	[summ] [numeric](13, 2) NOT NULL,
	[docpack] [int] NOT NULL,
	[date_1c] [date] NULL,
	[id_structur1] [int] NOT NULL,
	[id_structur2] [int] NOT NULL,
	[id_perf] [int] NOT NULL,
	[id_build_place] [int] NOT NULL,
	[doctext] [text] NOT NULL,
	[id_prj_code] [int] NOT NULL,
	[hiden] [tinyint] NOT NULL,
	[id_parent] [int] NOT NULL,
	[id_137] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_aunit]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_aunit](
	[id] [int] NOT NULL,
	[id_type] [int] NOT NULL,
	[id_item] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_build_place]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_build_place](
	[id] [int] NOT NULL,
	[place] [nvarchar](50) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_departs]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_departs](
	[id] [int] NOT NULL,
	[n_depart] [nvarchar](50) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_docname]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_docname](
	[id] [int] NOT NULL,
	[name] [nvarchar](254) NOT NULL,
	[id_doctree] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_doctree]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_doctree](
	[id] [int] NOT NULL,
	[id_parent] [int] NOT NULL,
	[treetext] [nvarchar](100) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_doctree_pre]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_doctree_pre](
	[id] [int] NOT NULL,
	[treetext] [nvarchar](100) NOT NULL,
	[tree_loaded] [bit] NOT NULL,
	[tree_parent] [int] NOT NULL,
	[tree_level] [int] NOT NULL,
	[tree_leaf] [bit] NOT NULL,
	[pos] [int] NOT NULL,
	[top_parent] [int] NOT NULL,
	[tree_expanded] [bit] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_doctype]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_doctype](
	[id] [int] NOT NULL,
	[name] [nvarchar](100) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_docversion]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_docversion](
	[id] [int] NOT NULL,
	[id_archive] [int] NOT NULL,
	[nn] [int] NOT NULL,
	[date_reg] [date] NULL,
	[id_104] [int] NOT NULL,
	[id_correct] [int] NOT NULL,
	[path] [nvarchar](250) NOT NULL,
	[main] [tinyint] NOT NULL,
	[prim] [nvarchar](250) NOT NULL,
	[id_user] [int] NOT NULL,
	[date_upd] [datetime] NULL,
	[barcode] [numeric](10, 0) NOT NULL,
	[id_user_t] [int] NOT NULL,
	[file_size] [int] NOT NULL,
	[id_138] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_frm]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_frm](
	[id] [int] NOT NULL,
	[name] [nvarchar](120) NOT NULL,
	[inn] [nvarchar](15) NOT NULL,
	[name_full] [nvarchar](254) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_persons]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_persons](
	[id] [int] NOT NULL,
	[short_name] [nvarchar](25) NOT NULL,
	[a] [tinyint] NOT NULL,
	[d] [tinyint] NOT NULL,
	[r] [tinyint] NOT NULL,
	[m] [tinyint] NOT NULL,
	[e] [tinyint] NOT NULL,
	[s] [tinyint] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_prj_code]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_prj_code](
	[id] [int] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[prj_name] [nvarchar](254) NOT NULL,
	[id_frm2] [int] NOT NULL,
	[dogovor] [nvarchar](150) NOT NULL,
	[prim] [nvarchar](254) NOT NULL,
	[prj_head] [nvarchar](50) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_registry]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_registry](
	[id] [int] NOT NULL,
	[date] [date] NULL,
	[id_docplace] [int] NOT NULL,
	[id_sender] [int] NOT NULL,
	[id_recipient] [int] NOT NULL,
	[prim] [nvarchar](250) NOT NULL,
	[date_upd] [datetime] NULL,
	[id_sklad1] [int] NOT NULL,
	[id_user] [int] NOT NULL,
	[id_135] [int] NOT NULL,
	[id_archive] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_registry_arc]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_registry_arc](
	[id] [int] NOT NULL,
	[id_registry] [int] NOT NULL,
	[id_archive] [int] NOT NULL,
	[copies] [int] NOT NULL,
	[prim] [nvarchar](250) NOT NULL,
	[id_104] [int] NOT NULL,
	[id_docversion] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_signs]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_signs](
	[id] [int] NOT NULL,
	[id_archive] [int] NOT NULL,
	[id_persons] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_structur]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_structur](
	[id] [int] NOT NULL,
	[id_parent] [int] NOT NULL,
	[name] [nvarchar](100) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_structur_list]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_structur_list](
	[id] [int] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[id_parent] [int] NOT NULL,
	[lvl] [nvarchar](100) NOT NULL,
	[inlist] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_structur_pre]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_structur_pre](
	[id] [int] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[tree_loaded] [bit] NOT NULL,
	[tree_parent] [int] NOT NULL,
	[tree_level] [int] NOT NULL,
	[tree_leaf] [bit] NOT NULL,
	[pos] [int] NOT NULL,
	[top_parent] [int] NOT NULL,
	[tree_expanded] [bit] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_users]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_users](
	[id] [int] NOT NULL,
	[short_name] [nvarchar](25) NOT NULL,
	[a] [tinyint] NOT NULL,
	[d] [tinyint] NOT NULL,
	[r] [tinyint] NOT NULL,
	[m] [tinyint] NOT NULL,
	[e] [tinyint] NOT NULL,
	[s] [tinyint] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[zao_stg_util]    Script Date: 18.06.2014 13:24:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[zao_stg_util](
	[id_1] [int] NOT NULL,
	[param_1] [nvarchar](20) NOT NULL,
	[id_10] [int] NOT NULL,
	[param_10] [nvarchar](20) NOT NULL,
	[id_13] [int] NOT NULL,
	[param_13] [nvarchar](20) NOT NULL,
	[id_104] [int] NOT NULL,
	[param_104] [nvarchar](20) NOT NULL,
	[id_131] [int] NOT NULL,
	[param_131] [nvarchar](20) NOT NULL,
	[id_133] [int] NOT NULL,
	[param_133] [nvarchar](15) NOT NULL,
	[id_135] [int] NOT NULL,
	[param_135] [nvarchar](20) NOT NULL,
	[id_137] [int] NOT NULL,
	[param_137] [nvarchar](30) NOT NULL,
	[id_138] [int] NOT NULL,
	[param_138] [nvarchar](30) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Index [IX_a_departs_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_a_departs_id] ON [dbo].[zao_stg_a_departs]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_a_departs_id_archive]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_a_departs_id_archive] ON [dbo].[zao_stg_a_departs]
(
	[id_archive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_a_departs_id_departs]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_a_departs_id_departs] ON [dbo].[zao_stg_a_departs]
(
	[id_departs] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_date_1c]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_date_1c] ON [dbo].[zao_stg_archive]
(
	[date_1c] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_date_doc]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_date_doc] ON [dbo].[zao_stg_archive]
(
	[date_doc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_date_upd]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_date_upd] ON [dbo].[zao_stg_archive]
(
	[date_upd] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_docpack]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_docpack] ON [dbo].[zao_stg_archive]
(
	[docpack] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_hiden]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_hiden] ON [dbo].[zao_stg_archive]
(
	[hiden] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id] ON [dbo].[zao_stg_archive]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_137]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_137] ON [dbo].[zao_stg_archive]
(
	[id_137] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_build_place]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_build_place] ON [dbo].[zao_stg_archive]
(
	[id_build_place] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_docname]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_docname] ON [dbo].[zao_stg_archive]
(
	[id_docname] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_doctype]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_doctype] ON [dbo].[zao_stg_archive]
(
	[id_doctype] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_frm2]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_frm2] ON [dbo].[zao_stg_archive]
(
	[id_frm2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_parent]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_parent] ON [dbo].[zao_stg_archive]
(
	[id_parent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_perf]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_perf] ON [dbo].[zao_stg_archive]
(
	[id_perf] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_prj_code]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_prj_code] ON [dbo].[zao_stg_archive]
(
	[id_prj_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_structur1]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_structur1] ON [dbo].[zao_stg_archive]
(
	[id_structur1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_structur2]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_structur2] ON [dbo].[zao_stg_archive]
(
	[id_structur2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_id_user]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_id_user] ON [dbo].[zao_stg_archive]
(
	[id_user] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_archive_summ]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_archive_summ] ON [dbo].[zao_stg_archive]
(
	[summ] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [ui_zao_stg_archive_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE UNIQUE NONCLUSTERED INDEX [ui_zao_stg_archive_id] ON [dbo].[zao_stg_archive]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_aunit_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_aunit_id] ON [dbo].[zao_stg_aunit]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_aunit_id_item]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_aunit_id_item] ON [dbo].[zao_stg_aunit]
(
	[id_item] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_aunit_id_type]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_aunit_id_type] ON [dbo].[zao_stg_aunit]
(
	[id_type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_build_place_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_build_place_id] ON [dbo].[zao_stg_build_place]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_departs_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_departs_id] ON [dbo].[zao_stg_departs]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docname_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docname_id] ON [dbo].[zao_stg_docname]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docname_id_doctree]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docname_id_doctree] ON [dbo].[zao_stg_docname]
(
	[id_doctree] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_doctree_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_doctree_id] ON [dbo].[zao_stg_doctree]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_doctree_id_parent]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_doctree_id_parent] ON [dbo].[zao_stg_doctree]
(
	[id_parent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_doctree_pre_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_doctree_pre_id] ON [dbo].[zao_stg_doctree_pre]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_doctree_pre_tree_parent]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_doctree_pre_tree_parent] ON [dbo].[zao_stg_doctree_pre]
(
	[tree_parent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_doctype_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_doctype_id] ON [dbo].[zao_stg_doctype]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_barcode]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_barcode] ON [dbo].[zao_stg_docversion]
(
	[barcode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_date_reg]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_date_reg] ON [dbo].[zao_stg_docversion]
(
	[date_reg] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_id] ON [dbo].[zao_stg_docversion]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_id_104]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_id_104] ON [dbo].[zao_stg_docversion]
(
	[id_104] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_id_138]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_id_138] ON [dbo].[zao_stg_docversion]
(
	[id_138] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_id_archive]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_id_archive] ON [dbo].[zao_stg_docversion]
(
	[id_archive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_id_correct]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_id_correct] ON [dbo].[zao_stg_docversion]
(
	[id_correct] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_id_user]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_id_user] ON [dbo].[zao_stg_docversion]
(
	[id_user] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_id_user_t]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_id_user_t] ON [dbo].[zao_stg_docversion]
(
	[id_user_t] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_docversion_nn]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_docversion_nn] ON [dbo].[zao_stg_docversion]
(
	[nn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_frm_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_frm_id] ON [dbo].[zao_stg_frm]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_frm_name]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_frm_name] ON [dbo].[zao_stg_frm]
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_persons_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_persons_id] ON [dbo].[zao_stg_persons]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_prj_code_dogovor]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_prj_code_dogovor] ON [dbo].[zao_stg_prj_code]
(
	[dogovor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_prj_code_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_prj_code_id] ON [dbo].[zao_stg_prj_code]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_prj_code_id_frm2]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_prj_code_id_frm2] ON [dbo].[zao_stg_prj_code]
(
	[id_frm2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_prj_code_name]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_prj_code_name] ON [dbo].[zao_stg_prj_code]
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_prj_code_prj_head]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_prj_code_prj_head] ON [dbo].[zao_stg_prj_code]
(
	[prj_head] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_date]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_date] ON [dbo].[zao_stg_registry]
(
	[date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_date_upd]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_date_upd] ON [dbo].[zao_stg_registry]
(
	[date_upd] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_id] ON [dbo].[zao_stg_registry]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_id_135]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_id_135] ON [dbo].[zao_stg_registry]
(
	[id_135] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_id_archive]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_id_archive] ON [dbo].[zao_stg_registry]
(
	[id_archive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_id_docplace]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_id_docplace] ON [dbo].[zao_stg_registry]
(
	[id_docplace] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_id_recipient]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_id_recipient] ON [dbo].[zao_stg_registry]
(
	[id_recipient] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_id_sender]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_id_sender] ON [dbo].[zao_stg_registry]
(
	[id_sender] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_id_sklad1]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_id_sklad1] ON [dbo].[zao_stg_registry]
(
	[id_sklad1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_id_user]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_id_user] ON [dbo].[zao_stg_registry]
(
	[id_user] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_arc_copies]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_arc_copies] ON [dbo].[zao_stg_registry_arc]
(
	[copies] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_arc_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_arc_id] ON [dbo].[zao_stg_registry_arc]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_arc_id_104]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_arc_id_104] ON [dbo].[zao_stg_registry_arc]
(
	[id_104] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_arc_id_archive]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_arc_id_archive] ON [dbo].[zao_stg_registry_arc]
(
	[id_archive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_arc_id_docversion]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_arc_id_docversion] ON [dbo].[zao_stg_registry_arc]
(
	[id_docversion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_registry_arc_id_registry]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_registry_arc_id_registry] ON [dbo].[zao_stg_registry_arc]
(
	[id_registry] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_signs_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_signs_id] ON [dbo].[zao_stg_signs]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_signs_id_archive]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_signs_id_archive] ON [dbo].[zao_stg_signs]
(
	[id_archive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_signs_id_persons]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_signs_id_persons] ON [dbo].[zao_stg_signs]
(
	[id_persons] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_structur_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_structur_id] ON [dbo].[zao_stg_structur]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_structur_id_parent]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_structur_id_parent] ON [dbo].[zao_stg_structur]
(
	[id_parent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_structur_pre_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_structur_pre_id] ON [dbo].[zao_stg_structur_pre]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_structur_pre_tree_parent]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_structur_pre_tree_parent] ON [dbo].[zao_stg_structur_pre]
(
	[tree_parent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_users_id]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_users_id] ON [dbo].[zao_stg_users]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_util_id_1]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_id_1] ON [dbo].[zao_stg_util]
(
	[id_1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_util_id_10]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_id_10] ON [dbo].[zao_stg_util]
(
	[id_10] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_util_id_104]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_id_104] ON [dbo].[zao_stg_util]
(
	[id_104] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_util_id_13]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_id_13] ON [dbo].[zao_stg_util]
(
	[id_13] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_util_id_131]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_id_131] ON [dbo].[zao_stg_util]
(
	[id_131] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_util_id_133]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_id_133] ON [dbo].[zao_stg_util]
(
	[id_133] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_util_id_135]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_id_135] ON [dbo].[zao_stg_util]
(
	[id_135] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_util_id_137]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_id_137] ON [dbo].[zao_stg_util]
(
	[id_137] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_util_id_138]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_id_138] ON [dbo].[zao_stg_util]
(
	[id_138] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_util_param_138]    Script Date: 18.06.2014 13:24:34 ******/
CREATE NONCLUSTERED INDEX [IX_util_param_138] ON [dbo].[zao_stg_util]
(
	[param_138] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
