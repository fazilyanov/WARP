USE [Archive]
GO

/****** Object:  View [dbo].[test_archive_depart_view]    Script Date: 06.02.2015 16:24:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
--DECLARE @base as varchar(MAX) = 'test' --1
DECLARE @base as varchar(MAX) = 'zao_stg' --1
--DECLARE @base as varchar(MAX) = 'sib' --2
--DECLARE @base as varchar(MAX) = 'west' --3
--DECLARE @base as varchar(MAX) = 'ngm' --4
--DECLARE @base as varchar(MAX) = 'north' --5
--DECLARE @base as varchar(MAX) = 'pvs' --6
--DECLARE @base as varchar(MAX) = 'asm' --7
--DECLARE @base as varchar(MAX) = 'region' --8
--DECLARE @base as varchar(MAX) = 'autotrans' --9
--DECLARE @base as varchar(MAX) = 'complect' --10
--DECLARE @base as varchar(MAX) = 'logistic' --11
--DECLARE @base as varchar(MAX) = 'south' --12
--DECLARE @base as varchar(MAX) = 'energo' --13


;DECLARE @sql as varchar(MAX) ;
set @sql='ALTER TABLE dbo.'+@base+'_complectnew ADD
	perf_mail bit NOT NULL CONSTRAINT DF_'+@base+'_complectnew_perf_mail DEFAULT 0,
	buh_mail bit NOT NULL CONSTRAINT DF_'+@base+'_complectnew_buh_mail DEFAULT 0,
	oc_mail bit NOT NULL CONSTRAINT DF_'+@base+'_complectnew_oc_mail DEFAULT 0,
	term_mail bit NOT NULL CONSTRAINT DF_'+@base+'_complectnew_term_mail DEFAULT 0';
EXEC(@sql);



