using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web.Script.Serialization;

namespace WARP
{
    public partial class Archive : System.Web.UI.Page
    {
        public string htmlTableColumns;
        public string jsTableColumns;
        public string jsTableLanguage;
        public string curPage;
        public string documentTitle;

        protected void Page_Load(object sender, EventArgs e)
        {
            curPage = (Page.RouteData.Values["pPage"] ?? "").ToString().Trim();
            if (curPage.Length > 0)
            {
                //curPage = ComFunc.GetArchivePageByName(pPage);
                documentTitle = ComFunc.GetArchivePageNameRus(curPage);
                TableData tableData = InitTableData();
                htmlTableColumns = ComFunc.GenerateHtmlTableColumns(tableData);
                jsTableColumns = ComFunc.GenerateJSTableColumns(tableData);
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

        public static DataTable GetData(string curBase, string curTable, string archivePage, int displayStart, int displayLength, string sortCol, string sortDir)
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("SELECT * FROM  (");

            sbQuery.AppendLine("   SELECT COUNT(*) OVER() recordsFiltered");
            sbQuery.AppendLine("   ,T.ID");
            sbQuery.AppendLine("   ,T.NumDoc");
            sbQuery.AppendLine("   ,T.DocDate");
            sbQuery.AppendLine("   ,T.DateUpd");
            sbQuery.AppendLine("   ,T.Prim");
            sbQuery.AppendLine("   ,T.DocContent");
            sbQuery.AppendLine("   ,F.Name as FrmContr");
            sbQuery.AppendLine("   ,T.Summ");
            sbQuery.AppendLine("   ,T.DocPack");
            sbQuery.AppendLine("   FROM [dbo].[" + curBase + "Archive] T");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[Frm] F on T.IdFrmContr = F.ID");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE a.id>100000");
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
            int recordsFiltered = 0;
            TableData tableData = InitTableData();
            string sortCol = tableData.ColumnList.Count >= iSortCol ? tableData.ColumnList[iSortCol].NameSql : "";
            DataTable dt = GetData(curBase, curTable, archivePage, displayStart, displayLength, sortCol, sortDir);

            if (dt != null)
            {
                List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                CultureInfo ruRu = CultureInfo.CreateSpecificCulture("ru-RU");

                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();

                    foreach (TableColumn column in tableData.ColumnList)
                    {
                        switch (column.Type)
                        {
                            case TableColumnType.Integer:
                                row.Add(column.NameSql, Convert.ToInt32(dr[column.NameSql]));
                                break;

                            case TableColumnType.Money:
                                row.Add(column.NameSql, String.Format(ruRu, "{0:0,0.00}", Convert.ToDecimal(dr[column.NameSql])));
                                break;

                            case TableColumnType.DateTime:
                                row.Add(column.NameSql, ((DateTime)dr[column.NameSql]).ToString("dd.MM.yyyy HH:mm:ss"));
                                break;

                            case TableColumnType.Date:
                                row.Add(column.NameSql, ((DateTime)dr[column.NameSql]).ToString("dd.MM.yyyy"));
                                break;

                            case TableColumnType.String:
                            default:
                                row.Add(column.NameSql, dr[column.NameSql].ToString());
                                break;
                        }
                    }

                    recordsFiltered = Convert.ToInt32(dr["recordsFiltered"]);
                    data.Add(row);
                }
                var result = new
                {
                    draw,
                    recordsTotal = (int)ComFunc.ExecuteScalar("SELECT COUNT(*) FROM [dbo].[" + curBase + curTable+"]"),
                    recordsFiltered = recordsFiltered,
                    data = data
                };

                JavaScriptSerializer js = new JavaScriptSerializer();
                ret = js.Serialize(result);
            }
            return ret;
        }
    }
}