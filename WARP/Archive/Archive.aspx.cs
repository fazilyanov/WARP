using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace WARP
{
    public class TableDataArchive : TableData
    {
        public TableDataArchive()
        {

            ColumnList = new List<TableColumn>()
            {
                new TableColumn {
                    ViewCaption = "Код ЭА",
                    DataNameSql = "Id",
                    DataType = TableColumnType.Integer,
                    ViewWidth = 30,
                    EditType = TableColumnEditType.None,
                },
                new TableColumn {
                    ViewCaption = "Дата редак.",
                    DataNameSql = "DateUpd",
                    DataType = TableColumnType.DateTime,
                    ViewWidth = 30,
                    ViewAlign = TableColumnAlign.Center
                },
                new TableColumn {
                    ViewCaption = "Оператор",
                    DataNameSql = "User",
                    //FilterType = TableColumnFilterType.Autocomplete,
                    ViewWidth = 40,
                    DataLookUpTable = "User"
                },
                new TableColumn {
                    ViewCaption = "Номер документа",
                    DataNameSql = "NumDoc",
                    ViewWidth = 300,
                    FilterType = TableColumnFilterType.String,
                    EditType = TableColumnEditType.String,
                    EditRequired = true,
                    EditMax = 250,
                },
                new TableColumn {
                    ViewCaption = "Документ",
                    DataNameSql = "DocTree",
                    FilterType = TableColumnFilterType.Autocomplete,
                    ViewWidth = 150,
                    DataLookUpTable = "DocTree",
                    EditRequired = true,
                },
                new TableColumn {
                    ViewCaption = "Вид документа",
                    DataNameSql = "DocType",
                    FilterType = TableColumnFilterType.DropDown,
                    ViewWidth = 150,
                    DataLookUpTable = "DocType",
                    //EditType = TableColumnEditType.DropDown,
                    EditRequired = true,
                },
                new TableColumn {
                    ViewCaption = "Дата докум.",
                    DataNameSql = "DocDate",
                    DataType = TableColumnType.Date,
                    ViewWidth = 85,
                    ViewAlign = TableColumnAlign.Center,
                    EditRequired = true,
                },
                new TableColumn {
                    ViewCaption = "Содержание",
                    DataNameSql = "DocContent",
                    ViewWidth = 300,
                    FilterType = TableColumnFilterType.String,
                    EditType = TableColumnEditType.String,
                    EditMax = 250,
                },
                new TableColumn
                {
                    ViewCaption = "Контрагент",
                    DataNameSql = "FrmContr",
                    ViewWidth = 250,
                    FilterType = TableColumnFilterType.Autocomplete,
                    DataLookUpTable = "Frm",
                    //EditType = TableColumnEditType.Autocomplete,
                    EditRequired = true,
                },
                new TableColumn {
                    ViewCaption = "Сумма",
                    DataNameSql = "Summ",
                    DataType = TableColumnType.Money,
                    ViewWidth = 100,
                    ViewAlign = TableColumnAlign.Right,
                    //EditType = TableColumnEditType.Money,
                    EditDefaultText = "0.00",
                },
                new TableColumn {
                    ViewCaption = "Пакет",
                    DataNameSql = "DocPack",
                    DataType = TableColumnType.Integer,
                    ViewWidth = 50,
                    ViewAlign = TableColumnAlign.Center,
                    //EditType = TableColumnEditType.Integer,
                    EditDefaultText = "0",
                },
                new TableColumn {
                    ViewCaption = "Примечание",
                    DataNameSql = "Prim",
                    ViewWidth = 300,
                    FilterType = TableColumnFilterType.String,
                    EditType = TableColumnEditType.String,
                    EditMax = 250,
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
            sbQuery.AppendLine("   ,T.Active");
            sbQuery.AppendLine("   ,T.Del");
            sbQuery.AppendLine("   ,T.DateUpd");
            sbQuery.AppendLine("   ,T.IdUser");
            //
            sbQuery.AppendLine("   ,T.NumDoc");
            sbQuery.AppendLine("   ,T.DocDate");
            sbQuery.AppendLine("   ,T.IdDocType");
            sbQuery.AppendLine("   ,DT.Name as DocType");
            sbQuery.AppendLine("   ,T.IdDocTree");
            sbQuery.AppendLine("   ,DT2.Name as DocTree");
            sbQuery.AppendLine("   ,U.Name as [User]");
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
    }

    public partial class Archive : System.Web.UI.Page
    {
        public TableDataArchive tableData;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            string curPage = (Page.RouteData.Values["pPage"] ?? "").ToString().Trim();
            if (curPage.Length > 0)
            {
                tableData = new TableDataArchive();
                tableData.Init(Master.curBaseName, "Archive", curPage);
                tableData.BrowserTabTitle = ComFunc.GetArchivePageNameRus(curPage);
                tableData.PageTitle = "Электронный архив | База: " + Master.curBaseNameRus + " | Документы | " + tableData.BrowserTabTitle;
            }
            else
            {
                Response.Write("bad param");
                Response.End();
            }
        }
    }
}