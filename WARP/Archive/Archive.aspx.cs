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
        public string browserTabTitle;
        public string curPage;
        public string documentTitle;
        public TableData tableData;

        public static DataTable GetData(string curBase, string curTable, string archivePage, TableData tableData, int displayStart, int displayLength, string sortCol, string sortDir, string ids = "")
        {
            StringBuilder sbQuery = new StringBuilder();
            string sWhere = tableData.GenerateWhereClause();

            sbQuery.AppendLine("DECLARE @recordsFiltered int;");
            sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
            sbQuery.AppendLine("FROM [dbo].[" + curBase + curTable + "] a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine("	a.Del=0");
            sbQuery.AppendLine(sWhere);
            if (ids.Length > 0)
            {
                sbQuery.AppendLine(" AND a.id in (" + ids + ")");
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
            if (ids.Length > 0)
            {
                sbQuery.AppendLine(" AND a.id in (" + ids + ")");
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

        public static string GetJsonData(string curBase, string curTable, string archivePage, int draw, int displayStart, int displayLength, int iSortCol, string sortDir, string ids = "")
        {
            string ret = "";
            TableData tableData = InitTableData(curBase, curTable, archivePage);
            string sortCol = tableData.ColumnList.Count >= iSortCol ? tableData.ColumnList[iSortCol].NameSql : "";
            DataTable dt = GetData(curBase, curTable, archivePage, tableData, displayStart, displayLength, sortCol, sortDir, ids);
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
                    Type = TableColumnType.Integer,
                    Width = 30,
                    EditType = TableColumnEditType.None,
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
                    //FilterType = TableColumnFilterType.Autocomplete,
                    Width = 150,
                    LookUpTable = "User"
                },
                new TableColumn {
                    Caption = "Номер документа",
                    NameSql = "NumDoc",
                    Width = 300,
                    FilterType = TableColumnFilterType.String,
                    EditType = TableColumnEditType.String,
                    EditRequired = true,
                    EditMax = 250,
                },
                new TableColumn {
                    Caption = "Документ",
                    NameSql = "DocTree",
                    FilterType = TableColumnFilterType.Autocomplete,
                    Width = 150,
                    LookUpTable = "DocTree",
                    EditRequired = true,
                },
                new TableColumn {
                    Caption = "Вид документа",
                    NameSql = "DocType",
                    FilterType = TableColumnFilterType.DropDown,
                    Width = 150,
                    LookUpTable = "DocType",
                    //EditType = TableColumnEditType.DropDown,
                    EditRequired = true,
                },
                new TableColumn {
                    Caption = "Дата докум.",
                    NameSql = "DocDate",
                    Type = TableColumnType.Date,
                    Width = 85,
                    Align = TableColumnAlign.Center,
                    EditRequired = true,
                },
                new TableColumn {
                    Caption = "Содержание",
                    NameSql = "DocContent",
                    Width = 300,
                    FilterType = TableColumnFilterType.String,
                    EditType = TableColumnEditType.String,
                    EditMax = 250,
                },
                new TableColumn
                {
                    Caption = "Контрагент",
                    NameSql = "FrmContr",
                    Width = 250,
                    FilterType = TableColumnFilterType.Autocomplete,
                    LookUpTable = "Frm",
                    //EditType = TableColumnEditType.Autocomplete,
                    EditRequired = true,
                },
                new TableColumn {
                    Caption = "Сумма",
                    NameSql = "Summ",
                    Type = TableColumnType.Money,
                    Width = 100,
                    Align = TableColumnAlign.Right,
                    //EditType = TableColumnEditType.Money,
                    EditDefaultText = "0.00",
                },
                new TableColumn {
                    Caption = "Пакет",
                    NameSql = "DocPack",
                    Type = TableColumnType.Integer,
                    Width = 50,
                    Align = TableColumnAlign.Center,
                    //EditType = TableColumnEditType.Integer,
                    EditDefaultText = "0",
                },
                new TableColumn {
                    Caption = "Примечание",
                    NameSql = "Prim",
                    Width = 300,
                    FilterType = TableColumnFilterType.String,
                    EditType = TableColumnEditType.String,
                    EditMax = 250,
                },
            };
            return tableData;
        }

        private static string CheckSave(string curBase, string curTable, string curPage, string action, Dictionary<string, List<RequestData>> rows)
        {

            // TODO : проверка при удлении на использование
            return string.Empty;
        }

        public static string SaveData(string curBase, string curTable, string curPage, string action, Dictionary<string, List<RequestData>> rows)
        {
            // Ответ
            string result = string.Empty;

            // AJAX|JSON
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            // Инитим нашу таблицу
            TableData tableData = InitTableData(curBase, curTable, curPage);

            // Чекаем на простые условия (обязательность, длинна и тд)
            if (action != "remove")
                result = tableData.Check(curBase, curTable, curPage, action, rows);

            // Чекаем на условия конкретной таблицы
            if (string.IsNullOrEmpty(result))
                result = CheckSave(curBase, curTable, curPage, action, rows);

            // Сохраняем
            if (string.IsNullOrEmpty(result))
                result = tableData.Save(curBase, curTable, curPage, action, rows);

            // Если все прошло гладко, отправляем гриду обновленные данные
            if (string.IsNullOrEmpty(result))
            {
                // Список редактируемых ID
                string ids = string.Empty;
                foreach (string key in rows.Keys) ids += key + ",";

                // Получаем обновленные данные по этим id из базы
                DataTable dt = GetData(curBase, curTable, curPage, tableData, 0, 500, "ID", "asc", ids.Substring(0, ids.Length - 1));

                // Возвращаем JSON
                if (dt != null)
                    result = javaScriptSerializer.Serialize(new { data = ComFunc.GetFormatData(tableData, dt) });
                else
                    result = javaScriptSerializer.Serialize(new { error = "Обновленные данные не получены" });
            }
            return result;
        }

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
    }
}