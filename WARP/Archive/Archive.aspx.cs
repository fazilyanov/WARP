using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Script.Serialization;

namespace WARP
{
    public partial class Archive : System.Web.UI.Page
    {
        public string curPage;
        public string browserTabTitle;
        public string documentTitle;
        public TableData tableData;

        protected void Page_Load(object sender, EventArgs e)
        {
            curPage = (Page.RouteData.Values["pPage"] ?? "").ToString().Trim();
            if (curPage.Length > 0)
            {
                browserTabTitle = ComFunc.GetArchivePageNameRus(curPage);
                documentTitle = "Электронный архив | База: " + Master.curBaseNameRus + " | Документы | " + browserTabTitle;
                tableData = InitTableData();
            }
            else
            {
                Response.Write("bad param");
                Response.End();
            }
        }

        public static TableData InitTableData()
        {
            TableData tableData = new TableData();
            tableData.TableSql = "Archive";
            tableData.ColumnList = new List<TableColumn>()
            {
                new TableColumn { Name ="ID", NameSql="ID", Type = TableColumnType.Integer, Width=30 },
                new TableColumn { Name ="Дата редак.", NameSql="DateUpd",Type = TableColumnType.DateTime, Width=110, Align=TableColumnAlign.Center },
                new TableColumn { Name ="Номер документа", NameSql="NumDoc", Width=300 },
                new TableColumn { Name ="Дата докум.", NameSql="DocDate",Type = TableColumnType.Date, Width=85, Align=TableColumnAlign.Center},
                new TableColumn { Name ="Содержание", NameSql="DocContent", Width=300 },
                new TableColumn { Name ="Контрагент", NameSql="FrmContr", Width=250 },
                new TableColumn { Name ="Сумма", NameSql="Summ", Type = TableColumnType.Money, Width=100, Align=TableColumnAlign.Right},
                new TableColumn { Name ="Пакет", NameSql="DocPack", Type = TableColumnType.Integer, Width=50,Align=TableColumnAlign.Center },
                new TableColumn { Name ="Примечание", NameSql="Prim", Width=300 },
            };
            return tableData;
        }

        public static DataTable GetData(string curBase, string curTable, string archivePage, TableData tableData, int displayStart, int displayLength, string sortCol, string sortDir)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.AppendLine("DECLARE @recordsFiltered int;");
            sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
            sbQuery.AppendLine("FROM [dbo].[" + curBase + "Archive] a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine("	a.Del=0");
            sbQuery.AppendLine("	AND a.id>100000");
            sbQuery.AppendLine(";");

            sbQuery.AppendLine("SELECT * FROM  (");
            sbQuery.AppendLine("   SELECT @recordsFiltered AS recordsFiltered");
            sbQuery.AppendLine("   ,T.ID");
            sbQuery.AppendLine("   ,T.NumDoc");
            sbQuery.AppendLine("   ,T.DocDate");
            sbQuery.AppendLine("   ,T.DateUpd");
            sbQuery.AppendLine("   ,T.Prim");
            sbQuery.AppendLine("   ,T.DocContent");
            sbQuery.AppendLine("   ,F.Name as FrmContr");
            sbQuery.AppendLine("   ,T.Summ");
            sbQuery.AppendLine("   ,T.DocPack");
            sbQuery.AppendLine("   ,T.Del");
            sbQuery.AppendLine("   FROM [dbo].[" + curBase + "Archive] T");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[Frm] F on T.IdFrmContr = F.ID");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine("	a.Del=0");
            sbQuery.AppendLine("	AND a.id>100000");
            sbQuery.AppendLine("ORDER BY " + sortCol + " " + sortDir);
            sbQuery.AppendLine("OFFSET @displayStart ROWS FETCH FIRST @displayLength ROWS ONLY");

            SqlParameter[] sqlParameterArray = {
                new SqlParameter { ParameterName = "@displayStart", SqlDbType = SqlDbType.Int, Value = displayStart },
                new SqlParameter { ParameterName = "@displayLength", SqlDbType = SqlDbType.Int, Value = displayLength }
            };

            DataTable dt = ComFunc.GetData(sbQuery.ToString(), sqlParameterArray);
            return dt;
        }

        public static string GetJsonData(string curBase, string curTable, string archivePage, int draw, int displayStart, int displayLength, int iSortCol, string sortDir)
        {
            string ret = "";
            TableData tableData = InitTableData();
            string sortCol = tableData.ColumnList.Count >= iSortCol ? tableData.ColumnList[iSortCol].NameSql : "";
            DataTable dt = GetData(curBase, curTable, archivePage, tableData, displayStart, displayLength, sortCol, sortDir);
            if (dt != null)
            {
                var result = new
                {
                    draw,
                    recordsTotal = (int)ComFunc.ExecuteScalar("SELECT COUNT(*) FROM [dbo].[" + curBase + curTable + "]"),
                    recordsFiltered = Convert.ToInt32(dt.Rows.Count > 0 ? dt.Rows[0]["recordsFiltered"] : 0),
                    data = ComFunc.GetFormatData(tableData, dt)
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                ret = js.Serialize(result);
            }
            return ret;
        }
    }
}