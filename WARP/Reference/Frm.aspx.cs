using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace WARP
{
    public partial class Frm : System.Web.UI.Page
    {
        public string curPage;
        public string browserTabTitle;
        public string documentTitle;
        public TableData tableData;

        protected void Page_Load(object sender, EventArgs e)
        {
            browserTabTitle = "Контрагенты";
            documentTitle = "Электронный архив | Справочники | " + browserTabTitle;
            tableData = InitTableData();
        }

        public static TableData InitTableData()
        {
            TableData tableData = new TableData();
            tableData.TableSql = "Frm";
            tableData.ColumnList = new List<TableColumn>()
            {
                new TableColumn { Name ="ID", NameSql="ID", Type = TableColumnType.Integer, Width=30 },
                new TableColumn { Name ="Наименование", NameSql="Name", Width=110},
                new TableColumn { Name ="Наименование полное", NameSql="NameFull", Width=110},
                new TableColumn { Name ="ИНН", NameSql="Inn", Width=110},
            };
            return tableData;
        }

        public static DataTable GetData(string curBase, string curTable, string archivePage, TableData tableData, int displayStart, int displayLength, string sortCol, string sortDir)
        {
            SqlParameter[] sqlParameterArray = {
                new SqlParameter { ParameterName = "@displayStart", SqlDbType = SqlDbType.Int, Value = displayStart },
                new SqlParameter { ParameterName = "@displayLength", SqlDbType = SqlDbType.Int, Value = displayLength }
            };

            DataTable dt = ComFunc.GetData(tableData.GetDefaultSql(curBase, curTable, sortCol, sortDir), sqlParameterArray);
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