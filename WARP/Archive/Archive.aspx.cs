using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace WARP
{
    // Шапка
    public class TableDataArchive : TableData
    {
        public TableDataArchive()
        {
            ShowRowInfoButtom = true;
            ColumnList = new List<TableColumn>()
            {
                new TableColumn {
                    DataNameSql         = "Id",
                    DataType            = TableColumnType.Integer,
                    EditType            = TableColumnEditType.None,
                    ViewCaption         = "Код ЭА",
                    ViewWidth           = 70,
                },
                new TableColumn {
                    DataNameSql         = "DateUpd",
                    DataType            = TableColumnType.DateTime,
                    ViewCaption         = "Дата редак.",
                    ViewWidth           = 115,
                    ViewAlign           = TableColumnAlign.Center
                },
                new TableColumn {
                    DataNameSql         = "IdUser",
                    DataType            = TableColumnType.LookUp,
                    DataLookUpResult    = "User",
                    DataLookUpTable     = "User",
                    ViewCaption         = "Оператор",
                    ViewWidth           = 125,
                },
                new TableColumn {
                    DataNameSql         = "NumDoc",
                    EditType            = TableColumnEditType.String,
                    EditRequired        = true,
                    EditMax             = 250,
                    FilterType          = TableColumnFilterType.String,
                    ViewCaption         = "Номер документа",
                    ViewWidth           = 300,
                },
                new TableColumn {
                    DataNameSql         = "IdDocTree",
                    DataType            = TableColumnType.LookUp,
                    DataLookUpResult    = "DocTree",
                    DataLookUpTable     = "DocTree",
                    //EditRequired        = true,
                    EditType            = TableColumnEditType.Autocomplete,
                    FilterType          = TableColumnFilterType.Autocomplete,
                    ViewCaption         = "Документ",
                    ViewWidth           = 150,
                },
                new TableColumn {
                    DataNameSql         = "IdDocType",
                    DataType            = TableColumnType.LookUp,
                    DataLookUpResult    = "DocType",
                    DataLookUpTable     = "DocType",
                  //  EditRequired        = true,
                    EditType            = TableColumnEditType.DropDown,
                    FilterType          = TableColumnFilterType.DropDown,
                    ViewCaption         = "Вид документа",
                    ViewWidth           = 150,
                },
                new TableColumn {
                    DataNameSql         = "DocDate",
                    DataType            = TableColumnType.Date,
                   // EditRequired        = true,
                    EditType            = TableColumnEditType.Date,
                    ViewCaption         = "Дата документа",
                    ViewCaptionShort    = "Дата докум.",
                    ViewWidth           = 85,
                    ViewAlign           = TableColumnAlign.Center,
                },
                new TableColumn {
                    DataNameSql         = "DocContent",
                    EditType            = TableColumnEditType.String,
                    EditMax             = 250,
                    FilterType          = TableColumnFilterType.String,
                    ViewCaption         = "Содержание",
                    ViewWidth           = 300,
                },
                new TableColumn
                {
                    DataNameSql         = "IdFrmContr",
                    DataType            = TableColumnType.LookUp,
                    DataLookUpResult    = "FrmContr",
                    DataLookUpTable     = "Frm",
                    EditType            = TableColumnEditType.Autocomplete,
                   // EditRequired        = true,
                    FilterType          = TableColumnFilterType.Autocomplete,
                    ViewWidth           = 250,
                    ViewCaption         = "Контрагент",
                },
                new TableColumn {
                    DataNameSql         = "Summ",
                    DataType            = TableColumnType.Money,
                    EditDefaultValue    = "0",
                    EditType            = TableColumnEditType.Money,
                    ViewCaption         = "Сумма",
                    ViewWidth           = 100,
                    ViewAlign           = TableColumnAlign.Right,
                },
                new TableColumn {
                    DataNameSql         = "DocPack",
                    DataType            = TableColumnType.Integer,
                    EditDefaultText     = "0",
                    ViewCaption         = "Пакет",
                    ViewWidth           = 50,
                    ViewAlign           = TableColumnAlign.Center,
                },
                new TableColumn {
                    DataNameSql         = "Prim",
                    EditType            = TableColumnEditType.String,
                    EditMax             = 250,
                    FilterType          = TableColumnFilterType.String,
                    ViewCaption         = "Примечание",
                    ViewWidth           = 300,
                },
                new TableColumn {
                    DataNameSql         = "Files",
                    DataType            = TableColumnType.Files,
                    EditType            = TableColumnEditType.Files,
                    ViewCaption         = "Файлы",
                },
                new TableColumn {
                    DataNameSql         = "DocText",
                    DataType            = TableColumnType.Text,
                    EditType            = TableColumnEditType.Text,
                    ViewCaption         = "Текст документа",
                },
            };
        }

        public override DataTable GetData(string ids = null)
        {
            StringBuilder sbQuery = new StringBuilder();
            // Условия отборки
            StringBuilder sbWhere = new StringBuilder();

            if (!ShowDelRows)
                sbWhere.AppendLine("	a.Del=0 ");
            else
                sbWhere.AppendLine("	a.Del=1 ");

            if (!ShowNoneActiveRows)
                sbWhere.AppendLine("	AND a.Active=1 ");

            sbWhere.AppendLine(GenerateWhereClause());

            if (!string.IsNullOrEmpty(ids))
                sbWhere.AppendLine("    AND a.Id in (" + ids + ")");

            sbQuery.AppendLine("DECLARE @recordsFiltered int;");
            sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
            sbQuery.AppendLine("FROM [dbo].[" + SqlBase + TableSql + "] a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine(sbWhere.ToString());
            sbQuery.AppendLine(";");

            sbQuery.AppendLine("SELECT * FROM  (");
            sbQuery.AppendLine("   SELECT @recordsFiltered AS recordsFiltered");
            sbQuery.AppendLine("   ,T.Id");
            sbQuery.AppendLine("   ,T.IdVer");
            sbQuery.AppendLine("   ,T.Active");
            sbQuery.AppendLine("   ,T.Del");
            sbQuery.AppendLine("   ,T.DateUpd");
            sbQuery.AppendLine("   ,T.IdUser");
            sbQuery.AppendLine("   ,U.Name as [User]");
            //
            sbQuery.AppendLine("   ,T.NumDoc");
            sbQuery.AppendLine("   ,T.DocDate");
            sbQuery.AppendLine("   ,T.IdDocType");
            sbQuery.AppendLine("   ,DT.Name as DocType");
            sbQuery.AppendLine("   ,T.IdDocTree");
            sbQuery.AppendLine("   ,DT2.Name as DocTree");
            sbQuery.AppendLine("   ,T.Prim");
            sbQuery.AppendLine("   ,T.DocContent");
            sbQuery.AppendLine("   ,T.IdFrmContr");
            sbQuery.AppendLine("   ,F.Name as FrmContr");
            sbQuery.AppendLine("   ,T.Summ");
            sbQuery.AppendLine("   ,T.DocPack");
            sbQuery.AppendLine("   FROM [dbo].[" + SqlBase + TableSql + "] T");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[Frm] F on T.IdFrmContr = F.ID");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] U on T.IdUser = U.ID");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[DocType] DT on T.IdDocType = DT.ID");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[DocTree] DT2 on T.IdDocTree = DT2.ID");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine(sbWhere.ToString());
            sbQuery.AppendLine("ORDER BY a.[" + SortCol + "] " + SortDir);
            sbQuery.AppendLine("OFFSET @displayStart ROWS FETCH FIRST @displayLength ROWS ONLY");

            SqlParameter[] sqlParameterArray = {
                new SqlParameter { ParameterName = "@displayStart", SqlDbType = SqlDbType.Int, Value = DisplayStart },
                new SqlParameter { ParameterName = "@displayLength", SqlDbType = SqlDbType.Int, Value = DisplayLength }
            };

            DataTable dt = ComFunc.GetData(sbQuery.ToString(), sqlParameterArray);
            return dt;
        }

        public override string GenerateJSTableInfoButtonContent(string id)
        {
            DataTable dtFiles = GetFileList("0", id);
            StringBuilder ib = new StringBuilder();
            if (dtFiles.Rows.Count > 0)
            {
                foreach (DataRow row in dtFiles.Rows)
                {
                    ib.AppendLine("                 <button type=\"button\" class=\"btn btn-default\" onclick=\"window.open('/Handler/GetFileHandler.ashx?curBase=" + SqlBase + "&curTable=" + TableSql + "&IdFile=" + row["IdFile"].ToString() + "');\">");
                    ib.AppendLine(row["fileName"].ToString());
                    ib.AppendLine("                 </button>");
                }
            }
            else
            {
                ib.AppendLine("Файлов нет");
            }
            return ib.ToString();
        }
        public override string GenerateJSTableInfoButton()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("             var detailRows = [];");
            sb.AppendLine();
            sb.AppendLine("             function format(id) {return '<div id=\"RowInfo'+id+'\"></div>';}");
            sb.AppendLine();
            sb.AppendLine("             $('#table" + TableSql + " tbody').on( 'click', 'tr td.details-control', function () {");
            sb.AppendLine("                 var tr = $(this).closest('tr');");
            sb.AppendLine("                 var row = table.row(tr);");
            sb.AppendLine();
            sb.AppendLine("                 if (row.child.isShown()) {");
            sb.AppendLine("                     tr.removeClass('details');");
            sb.AppendLine("                     row.child.hide();");
            sb.AppendLine("                 }");
            sb.AppendLine("                 else {");
            sb.AppendLine("                     tr.addClass('details');");
            sb.AppendLine("                     row.child('<div id=\"RowInfo' + row.data().Id + '\">Загрузка..</div>').show();");

            sb.AppendLine("                     $('#RowInfo' + row.data().Id).load(");
            sb.AppendLine("                         '/Handler/InfoButtonHandler.ashx?curBase=" + SqlBase + "&curTable=" +TableSql + "&curPage=" + PageName + "&curId='+row.data().Id+'&_=' + (new Date()).getTime(), null,");
            sb.AppendLine("                         function(){");
            sb.AppendLine("                         }");
            sb.AppendLine("                     );");

            sb.AppendLine("                 }");
            sb.AppendLine("             });");
            sb.AppendLine();
            //sb.AppendLine("             table.on( 'draw', function () {");
            //sb.AppendLine("                 $.each( detailRows, function (i, id) {");
            //sb.AppendLine("                     $('#'+id+' td.details-control').trigger('click');");
            //sb.AppendLine("                 });");
            //sb.AppendLine("             });");
            return sb.ToString();
        }


    }
    //public override string GenerateJSTableInfoButton()
    //{
    //    StringBuilder sb = new StringBuilder();
    //    sb.AppendLine("             var detailRows = [];");
    //    sb.AppendLine();
    //    sb.AppendLine("             function format(d) {return 'skjdflksjlkjs';}");
    //    sb.AppendLine();
    //    sb.AppendLine("             $('#table" + TableSql + " tbody').on( 'click', 'tr td.details-control', function () {");
    //    sb.AppendLine("                 var tr = $(this).closest('tr');");
    //    sb.AppendLine("                 var row = table.row(tr);");
    //    sb.AppendLine("                 var idx = $.inArray( tr.attr('id'), detailRows );");
    //    sb.AppendLine();
    //    sb.AppendLine("                 if (row.child.isShown()) {");
    //    sb.AppendLine("                     tr.removeClass('details');");
    //    sb.AppendLine("                     row.child.hide();");
    //    sb.AppendLine("                     detailRows.splice( idx, 1 );");
    //    sb.AppendLine("                 }");
    //    sb.AppendLine("                 else {");
    //    sb.AppendLine("                     tr.addClass('details');");
    //    sb.AppendLine("                     row.child(format(row.data())).show();");
    //    sb.AppendLine();
    //    sb.AppendLine("                     if ( idx === -1 ) {");
    //    sb.AppendLine("                         detailRows.push(tr.attr('id'));");
    //    sb.AppendLine("                     }");
    //    sb.AppendLine("                 }");
    //    sb.AppendLine("             });");
    //    sb.AppendLine();
    //    sb.AppendLine("             table.on( 'draw', function () {");
    //    sb.AppendLine("                 $.each( detailRows, function (i, id) {");
    //    sb.AppendLine("                     $('#'+id+' td.details-control').trigger('click');");
    //    sb.AppendLine("                 });");
    //    sb.AppendLine("             });");
    //    return sb.ToString();
    //}

    // Страница
    public class AppPageArchive : AppPage
    {
        public AppPageArchive()
        {
            Master = new TableDataArchive();
        }
    }

    // ASPX
    public partial class Archive : System.Web.UI.Page
    {
        public AppPageArchive appPage;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            string curPage = (Page.RouteData.Values["pPage"] ?? "").ToString().Trim();
            if (curPage.Length > 0)
            {
                appPage = new AppPageArchive();
                appPage.Master.Init(Master.curBaseName, "Archive", curPage);
                appPage.BrowserTabTitle = ComFunc.GetArchivePageNameRus(curPage);
                appPage.Master.PageTitle = "Электронный архив | База: " + Master.curBaseNameRus + " | Документы | " + appPage.BrowserTabTitle;
                appPage.EditDialogWidth = 1007;
            }
            else
            {
                Response.Write("bad param");
                Response.End();
            }
        }
    }
}