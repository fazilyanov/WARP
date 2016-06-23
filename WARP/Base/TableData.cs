using System;
using System.Collections.Generic;
using System.Text;

namespace WARP
{
    public enum TableColumnType
    {
        String,
        Integer,
        Money,
        DateTime,
        Date,
    }

    public enum TableColumnFilterType
    {
        None,
        String,
        Integer,
        Money,
    }

    public enum TableColumnAlign
    {
        Left,
        Center,
        Right,
    }

    public class TableColumn
    {
        public string Name { get; set; } = string.Empty;
        public string NameSql { get; set; } = string.Empty;
        public TableColumnType Type { get; set; } = TableColumnType.String;
        public int Width { get; set; } = 100;
        public TableColumnAlign Align { get; set; } = TableColumnAlign.Left;
        public TableColumnFilterType FilterType { get; set; } = TableColumnFilterType.None;
        public string DefaultFilterValue { get; set; } = string.Empty;
    }

    public class TableData
    {
        public string TableSql { get; set; } = string.Empty;

        public List<TableColumn> ColumnList { get; set; } = null;

        public string GenerateHtmlTableColumns()
        {
            string ret = Environment.NewLine;
            foreach (TableColumn item in ColumnList)
            {
                ret += "                        <th>" + item.Name + "</th>" + Environment.NewLine;
            }
            return ret;
        }

        public string GenerateJSTableColumns()
        {
            string ret = Environment.NewLine;
            foreach (TableColumn item in ColumnList)
            {
                ret += "                    { \"data\": \"" + item.NameSql + "\", className:\"dt-body-" + item.Align.ToString().ToLower() + "\", \"width\": \"" + item.Width + "px\" }," + Environment.NewLine;
            }
            return ret;
        }

        public string GetDefaultSql(string curBase, string curTable, string sortCol, string sortDir)
        {
            StringBuilder sbWhere = new StringBuilder();
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("DECLARE @recordsFiltered int;");
            sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
            sbQuery.AppendLine("FROM [dbo].[" + curBase + curTable + "] a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine("	a.Del=0");
            sbQuery.AppendLine(sbWhere.ToString());

            sbQuery.AppendLine(";");

            sbQuery.AppendLine("SELECT * FROM  (");
            sbQuery.AppendLine("   SELECT @recordsFiltered AS recordsFiltered");
            foreach (TableColumn item in ColumnList)
            {
                sbQuery.AppendLine("   ,T." + item.NameSql);
            }
            sbQuery.AppendLine("   ,T.Del");
            sbQuery.AppendLine("   FROM [dbo].[" + curBase + curTable + "] T");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine("	a.Del=0");
            sbQuery.AppendLine(sbWhere.ToString());
            sbQuery.AppendLine("ORDER BY " + sortCol + " " + sortDir);
            sbQuery.AppendLine("OFFSET @displayStart ROWS FETCH FIRST @displayLength ROWS ONLY");

            return sbQuery.ToString();
        }
    }
}