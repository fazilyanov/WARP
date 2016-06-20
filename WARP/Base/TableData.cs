using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }

    public class TableData
    {
        public List<TableColumn> ColumnList { get; set; } = null;
    }
}