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
        }

        public override DataTable GetData(string ids = null)
        {
            StringBuilder sbQuery = new StringBuilder();
            string sWhere = GenerateWhereClause();

            sbQuery.AppendLine("DECLARE @recordsFiltered int;");
            sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
            sbQuery.AppendLine("FROM [dbo].[" + BaseSql + TableSql + "] a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine("	a.Del=0");
            sbQuery.AppendLine(sWhere);
            if (!string.IsNullOrEmpty(ids))
                sbQuery.AppendLine(" AND a.id in (" + ids + ")");
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
            sbQuery.AppendLine("   FROM [dbo].[" + BaseSql + TableSql + "] T");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[Frm] F on T.IdFrmContr = F.ID");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] U on T.IdUser = U.ID");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[DocType] DT on T.IdDocType = DT.ID");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[DocTree] DT2 on T.IdDocTree = DT2.ID");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine("	a.Del=0");
            sbQuery.AppendLine(sWhere);
            if (!string.IsNullOrEmpty(ids))
                sbQuery.AppendLine(" AND a.id in (" + ids + ")");

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
}