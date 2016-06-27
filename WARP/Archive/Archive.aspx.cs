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

        protected void Page_PreRender(object sender, EventArgs e)
        {
            curPage = (Page.RouteData.Values["pPage"] ?? "").ToString().Trim();
            if (curPage.Length > 0)
            {
                browserTabTitle = ComFunc.GetArchivePageNameRus(curPage);
                documentTitle = "Электронный архив | База: " + Master.curBaseNameRus + " | Документы | " + browserTabTitle;
                tableData = InitTableData(Master.curBaseName, "Archive", curPage);
            }
            else
            {
                Response.Write("bad param");
                Response.End();
            }
        }

        public static TableData InitTableData(string curBase, string curTable, string archivePage)
        {
            TableData tableData = new TableData();
            tableData.BaseSql = curBase;
            tableData.TableSql = curTable;
            tableData.PageName = archivePage;

            tableData.ColumnList = new List<TableColumn>()
            {
                new TableColumn {
                    Caption = "ID",
                    NameSql = "ID",
                    Type = TableColumnType.Integer, Width=30
                },
                new TableColumn {
                    Caption = "Дата редак.",
                    NameSql = "DateUpd",
                    Type = TableColumnType.DateTime,
                    Width = 110,
                    Align = TableColumnAlign.Center
                },
                new TableColumn {
                    Caption = "Оператор",
                    NameSql = "User",
                    FilterType = TableColumnFilterType.Autocomplete,
                    Width = 150,
                    LookUpTable = "User",
                },
                new TableColumn {
                    Caption = "Номер документа",
                    NameSql = "NumDoc",
                    Width = 300,
                    FilterType = TableColumnFilterType.String,
                },
                new TableColumn {
                    Caption = "Документ",
                    NameSql = "DocTree",
                    FilterType = TableColumnFilterType.Autocomplete,
                    Width = 150,
                    LookUpTable = "DocTree",
                },
                new TableColumn {
                    Caption = "Вид документа",
                    NameSql = "DocType",
                    FilterType = TableColumnFilterType.DropDown,
                    Width = 150,
                    LookUpTable = "DocType",
                },
                new TableColumn {
                    Caption = "Дата докум.",
                    NameSql = "DocDate",
                    Type = TableColumnType.Date,
                    Width = 85,
                    Align = TableColumnAlign.Center
                },
                new TableColumn {
                    Caption = "Содержание",
                    NameSql = "DocContent",
                    Width = 300,
                    FilterType = TableColumnFilterType.String,
                },
                new TableColumn
                {
                    Caption = "Контрагент",
                    NameSql = "FrmContr",
                    Width = 250,
                    FilterType = TableColumnFilterType.Autocomplete,
                    LookUpTable = "Frm",
                },
                new TableColumn {
                    Caption = "Сумма",
                    NameSql = "Summ",
                    Type = TableColumnType.Money,
                    Width = 100,
                    Align = TableColumnAlign.Right
                },
                new TableColumn {
                    Caption = "Пакет",
                    NameSql = "DocPack",
                    Type = TableColumnType.Integer,
                    Width = 50,
                    Align = TableColumnAlign.Center
                },
                new TableColumn {
                    Caption = "Примечание",
                    NameSql = "Prim",
                    Width = 300,
                    FilterType = TableColumnFilterType.String,
                },
            };
            return tableData;
        }

        public static bool SaveData(string curBase, string curTable, string curPage, string action, Dictionary<string, List<RequestData>> rows)
        {
            Dictionary<string, StringBuilder> query = new Dictionary<string, StringBuilder>();
            Dictionary<string, List<SqlParameter>> param = new Dictionary<string, List<SqlParameter>>();
            bool ret = false;
            TableData tableData = InitTableData(curBase, curTable, curPage);

            foreach (KeyValuePair<string, List<RequestData>> pair in rows)
            {
                query[pair.Key] = new StringBuilder();
                param[pair.Key] = new List<SqlParameter>();

                query[pair.Key].AppendLine("UPDATE[dbo].[" + curBase + curTable + "] SET");
                query[pair.Key].AppendLine("     [Del] = 0");
                foreach (RequestData rd in pair.Value)
                {
                    query[pair.Key].AppendLine("    ,[" + rd.FieldName + "] = @" + rd.FieldName);
                    param[pair.Key].Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.NVarChar, Value = rd.FieldValue });
                }
                query[pair.Key].AppendLine("WHERE ID = @ID");
                param[pair.Key].Add(new SqlParameter { ParameterName = "@ID", SqlDbType = SqlDbType.Int, Value = pair.Key });
                ComFunc.ExecuteNonQueryTransaction(query[pair.Key].ToString(), param[pair.Key].ToArray());
            }

            return ret;
        }

        public static DataTable GetData(string curBase, string curTable, string archivePage, TableData tableData, int displayStart, int displayLength, string sortCol, string sortDir, string where = "")
        {
            StringBuilder sbQuery = new StringBuilder();
            string sWhere = tableData.GenerateWhereClause();

            sbQuery.AppendLine("DECLARE @recordsFiltered int;");
            sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
            sbQuery.AppendLine("FROM [dbo].[" + curBase + curTable + "] a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine("	a.Del=0");
            sbQuery.AppendLine(sWhere);
            if (where.Length>0)
            {
                sbQuery.AppendLine(" AND a.id in ("+where+")");
            }
            sbQuery.AppendLine(";");

            sbQuery.AppendLine("SELECT * FROM  (");
            sbQuery.AppendLine("   SELECT @recordsFiltered AS recordsFiltered");
            sbQuery.AppendLine("   ,T.ID");
            sbQuery.AppendLine("   ,T.NumDoc");
            sbQuery.AppendLine("   ,T.DocDate");
            sbQuery.AppendLine("   ,T.IdDocType");
            sbQuery.AppendLine("   ,DT.Name as DocType");
            sbQuery.AppendLine("   ,T.IdDocTree");
            sbQuery.AppendLine("   ,DT2.Name as DocTree");
            sbQuery.AppendLine("   ,T.DateUpd");
            sbQuery.AppendLine("   ,T.IdUser");
            sbQuery.AppendLine("   ,U.Name as [User]");
            sbQuery.AppendLine("   ,T.Prim");
            sbQuery.AppendLine("   ,T.DocContent");
            sbQuery.AppendLine("   ,T.IdFrmContr");
            sbQuery.AppendLine("   ,F.Name as FrmContr");
            sbQuery.AppendLine("   ,T.Summ");
            sbQuery.AppendLine("   ,T.DocPack");
            sbQuery.AppendLine("   ,T.Del");
            sbQuery.AppendLine("   FROM [dbo].[" + curBase + "Archive] T");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[Frm] F on T.IdFrmContr = F.ID");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] U on T.IdUser = U.ID");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[DocType] DT on T.IdDocType = DT.ID");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[DocTree] DT2 on T.IdDocTree = DT2.ID");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine("	a.Del=0");
            sbQuery.AppendLine(sWhere);
            if (where.Length > 0)
            {
                sbQuery.AppendLine(" AND a.id in (" + where + ")");
            }
            sbQuery.AppendLine("ORDER BY a.[" + sortCol + "] " + sortDir);
            sbQuery.AppendLine("OFFSET @displayStart ROWS FETCH FIRST @displayLength ROWS ONLY");

            SqlParameter[] sqlParameterArray = {
                new SqlParameter { ParameterName = "@displayStart", SqlDbType = SqlDbType.Int, Value = displayStart },
                new SqlParameter { ParameterName = "@displayLength", SqlDbType = SqlDbType.Int, Value = displayLength }
            };

            DataTable dt = ComFunc.GetData(sbQuery.ToString(), sqlParameterArray);
            return dt;
        }

        public static string GetJsonData(string curBase, string curTable, string archivePage, int draw, int displayStart, int displayLength, int iSortCol, string sortDir, string where = "")
        {
            string ret = "";
            TableData tableData = InitTableData(curBase, curTable, archivePage);
            string sortCol = tableData.ColumnList.Count >= iSortCol ? tableData.ColumnList[iSortCol].NameSql : "";
            DataTable dt = GetData(curBase, curTable, archivePage, tableData, displayStart, displayLength, sortCol, sortDir, where);
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