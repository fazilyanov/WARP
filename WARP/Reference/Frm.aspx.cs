using System;
using System.Collections.Generic;

namespace WARP
{
    public class TableDataFrm : TableData
    {
        public TableDataFrm()
        {
            ColumnList = new List<TableColumn>()
            {
                new TableColumn {
                    Caption = "ID",
                    NameSql = "ID",
                    Type = TableColumnType.Integer,
                    Width = 30,
                },
                new TableColumn {
                    Caption = "Дата редак.",
                    NameSql = "DateUpd",
                    Type = TableColumnType.DateTime,
                    Width = 30,
                    Align = TableColumnAlign.Center
                },
                new TableColumn {
                    Caption = "Оператор",
                    NameSql = "User",
                    FilterType = TableColumnFilterType.Autocomplete,
                    Width = 40,
                    LookUpTable = "User"
                },
                new TableColumn {
                    Caption = "Наименование",
                    NameSql = "Name",
                    FilterType = TableColumnFilterType.String,
                    Width = 150,
                    EditType=TableColumnEditType.String,
                    EditRequired = true,
                    EditMax = 120,
                },
                new TableColumn {
                    Caption = "Наименование полное",
                    NameSql = "NameFull",
                    FilterType = TableColumnFilterType.String,
                    Width = 150,
                    EditRequired = true,
                    EditType=TableColumnEditType.String,
                    EditMax = 250,
                },
                new TableColumn {
                    Caption = "ИНН",
                    NameSql = "Inn",
                    FilterType = TableColumnFilterType.String,
                    Width = 40,
                    EditRequired = true,
                    EditType=TableColumnEditType.String,
                    EditMax = 15,
                },
            };
        }

        //public override DataTable GetData(string ids = null)
        //{
        //    StringBuilder sbQuery = new StringBuilder();
        //    string sWhere = GenerateWhereClause();

        //    sbQuery.AppendLine("DECLARE @recordsFiltered int;");
        //    sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
        //    sbQuery.AppendLine("FROM [dbo].[" + BaseSql + TableSql + "] a");
        //    sbQuery.AppendLine("WHERE");
        //    sbQuery.AppendLine("	a.Del=0");
        //    sbQuery.AppendLine(sWhere);
        //    if (!string.IsNullOrEmpty(ids))
        //        sbQuery.AppendLine(" AND a.id in (" + ids + ")");
        //    sbQuery.AppendLine(";");

        //    sbQuery.AppendLine("SELECT * FROM  (");
        //    sbQuery.AppendLine("   SELECT @recordsFiltered AS recordsFiltered");
        //    sbQuery.AppendLine("   ,T.ID");
        //    sbQuery.AppendLine("   ,T.NumDoc");
        //    sbQuery.AppendLine("   ,T.DocDate");
        //    sbQuery.AppendLine("   ,T.IdDocType");
        //    sbQuery.AppendLine("   ,DT.Name as DocType");
        //    sbQuery.AppendLine("   ,T.IdDocTree");
        //    sbQuery.AppendLine("   ,DT2.Name as DocTree");
        //    sbQuery.AppendLine("   ,T.DateUpd");
        //    sbQuery.AppendLine("   ,T.IdUser");
        //    sbQuery.AppendLine("   ,U.Name as [User]");
        //    sbQuery.AppendLine("   ,T.Prim");
        //    sbQuery.AppendLine("   ,T.DocContent");
        //    sbQuery.AppendLine("   ,T.IdFrmContr");
        //    sbQuery.AppendLine("   ,F.Name as FrmContr");
        //    sbQuery.AppendLine("   ,T.Summ");
        //    sbQuery.AppendLine("   ,T.DocPack");
        //    sbQuery.AppendLine("   ,T.Del");
        //    sbQuery.AppendLine("   FROM [dbo].[" + BaseSql + TableSql + "] T");
        //    sbQuery.AppendLine("   LEFT JOIN [dbo].[Frm] F on T.IdFrmContr = F.ID");
        //    sbQuery.AppendLine("   LEFT JOIN [dbo].[User] U on T.IdUser = U.ID");
        //    sbQuery.AppendLine("   LEFT JOIN [dbo].[DocType] DT on T.IdDocType = DT.ID");
        //    sbQuery.AppendLine("   LEFT JOIN [dbo].[DocTree] DT2 on T.IdDocTree = DT2.ID");
        //    sbQuery.AppendLine(") a");
        //    sbQuery.AppendLine("WHERE");
        //    sbQuery.AppendLine("	a.Del=0");
        //    sbQuery.AppendLine(sWhere);
        //    if (!string.IsNullOrEmpty(ids))
        //        sbQuery.AppendLine(" AND a.id in (" + ids + ")");

        //    sbQuery.AppendLine("ORDER BY a.[" + SortCol + "] " + SortDir);
        //    sbQuery.AppendLine("OFFSET @displayStart ROWS FETCH FIRST @displayLength ROWS ONLY");

        //    SqlParameter[] sqlParameterArray = {
        //        new SqlParameter { ParameterName = "@displayStart", SqlDbType = SqlDbType.Int, Value = DisplayStart },
        //        new SqlParameter { ParameterName = "@displayLength", SqlDbType = SqlDbType.Int, Value = DisplayLength }
        //    };

        //    DataTable dt = ComFunc.GetData(sbQuery.ToString(), sqlParameterArray);
        //    return dt;
        //}
    }

    public partial class Frm : System.Web.UI.Page
    {
        public TableDataFrm tableData;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            tableData = new TableDataFrm();
            tableData.Init(Master.curBaseName, "Frm");
            tableData.BrowserTabTitle = "Контрагенты";
            tableData.PageTitle = "Электронный архив | Справочники | " + tableData.BrowserTabTitle;
        }
    }
}