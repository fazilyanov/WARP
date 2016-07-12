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
                new TableColumn { Caption ="ID", NameSql="ID", Type = TableColumnType.Integer, Width=30 },
                new TableColumn { Caption ="Наименование", NameSql="Name", Width=110},
                new TableColumn { Caption ="Наименование полное", NameSql="NameFull", Width=110},
                new TableColumn { Caption ="ИНН", NameSql="Inn", Width=110},
            };
            return tableData;
        }

               
    }
}