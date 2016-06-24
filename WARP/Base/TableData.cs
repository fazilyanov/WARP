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
        Autocomplete,
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
        public string Caption { get; set; } = string.Empty;
        public string CaptionShort { get; set; } = string.Empty;
        public string CaptionFilter { get; set; } = string.Empty;
        public string NameSql { get; set; } = string.Empty;
        public TableColumnType Type { get; set; } = TableColumnType.String;
        public int Width { get; set; } = 100;
        public TableColumnAlign Align { get; set; } = TableColumnAlign.Left;
        public TableColumnFilterType FilterType { get; set; } = TableColumnFilterType.None;
        public string DefaultFilterValue { get; set; } = string.Empty;
        public string LookUpTable { get; set; } = string.Empty;
        public string LookUpField { get; set; } = string.Empty;
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
                ret += "                        <th>" + item.Caption + "</th>" + Environment.NewLine;
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

        public string GenerateFilterFormDialog()
        {
            StringBuilder sbHtml = new StringBuilder();
            StringBuilder sbJS = new StringBuilder();

            sbHtml.AppendLine("    <div class=\"modal fade\" id=\"modalFilterForm\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"modalFilterForm\" aria-hidden=\"true\">");
            sbHtml.AppendLine("        <div class=\"modal-dialog modal-lg\">");
            sbHtml.AppendLine("            <div class=\"modal-content\">");
            sbHtml.AppendLine("                <div class=\"modal-header\">");
            sbHtml.AppendLine("                    <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\">&times;</button>");
            sbHtml.AppendLine("                    <h4 class=\"modal-title\">Установить фильтр</h4>");
            sbHtml.AppendLine("                </div>");
            sbHtml.AppendLine("                <div class=\"modal-body\">");
            foreach (TableColumn item in ColumnList)
            {
                switch (item.FilterType)
                {
                    case TableColumnFilterType.String:
                        break;

                    case TableColumnFilterType.Autocomplete:
                        sbHtml.AppendLine("                    <div class=\"row\">");
                        sbHtml.AppendLine("                        <div class=\"col-sm-3\">");
                        sbHtml.AppendLine("                            <h5>" + (string.IsNullOrEmpty(item.CaptionFilter) ? item.Caption : item.CaptionFilter) + "</h5>");
                        sbHtml.AppendLine("                        </div>");
                        sbHtml.AppendLine("                        <div class=\"col-sm-3\">");
                        sbHtml.AppendLine("                             <select id=\"cond"+ item.NameSql + "\" class=\"combobox form-control input-sm\">");
                        sbHtml.AppendLine("                                 <option></option>");
                        sbHtml.AppendLine("                                 <option value=\"=\" selected>Равно</option>");                        
                        sbHtml.AppendLine("                             </select>");
                        sbHtml.AppendLine("                        </div>");
                        sbHtml.AppendLine("                        <div class=\"col-sm-6\">");
                        sbHtml.AppendLine("                            <input type=\"text\" id=\"" + item.NameSql + "\" class=\"form-control input-sm\" placeholder=\"Начните вводить для поиска..\">");
                        sbHtml.AppendLine("                            <input type=\"hidden\" id=\"Id" + item.NameSql + "\">");
                        sbHtml.AppendLine("                        </div>");
                        sbHtml.AppendLine("                    </div>");
                        sbJS.AppendLine();
                        sbJS.AppendLine("            // Для столбца: " + item.Caption);
                        sbJS.AppendLine("            var source" + item.NameSql + " = new Bloodhound({");
                        sbJS.AppendLine("                datumTokenizer: Bloodhound.tokenizers.whitespace,");
                        sbJS.AppendLine("                queryTokenizer: Bloodhound.tokenizers.whitespace,");
                        sbJS.AppendLine("                remote: {");
                        sbJS.AppendLine("                    url: '/Handler/TypeaheadHandler.ashx?t=" + item.LookUpTable + "&q=%QUERY',");
                        sbJS.AppendLine("                    wildcard: '%QUERY'");
                        sbJS.AppendLine("                }");
                        sbJS.AppendLine("            });");

                        sbJS.AppendLine("            $('#" + item.NameSql + "').typeahead({");
                        sbJS.AppendLine("                minLength: 1,");
                        sbJS.AppendLine("                highlight: true");
                        sbJS.AppendLine("            },");
                        sbJS.AppendLine("            {");
                        sbJS.AppendLine("                name: 'th" + item.NameSql + "',");
                        sbJS.AppendLine("                display: 'Name',");
                        sbJS.AppendLine("                highlight: true,");
                        sbJS.AppendLine("                limit: 15,");
                        sbJS.AppendLine("                source: source" + item.NameSql + ",");
                        sbJS.AppendLine("            });");

                        sbJS.AppendLine("            $(\"#" + item.NameSql + "\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#Id" + item.NameSql + "\").val(datum.ID); });");
                        sbJS.AppendLine("            $('#cond"+ item.NameSql+"').combobox();");
                        break;

                    case TableColumnFilterType.Integer:
                        break;

                    case TableColumnFilterType.Money:
                        break;

                    default:
                        break;
                }
            }

            sbHtml.AppendLine("                </div>");
            sbHtml.AppendLine("                <div class=\"modal-footer\">");
            sbHtml.AppendLine("                    <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Закрыть</button>");
            sbHtml.AppendLine("                    <button type=\"button\" class=\"btn btn-primary\">Применить</button>");
            sbHtml.AppendLine("                </div>");
            sbHtml.AppendLine("            </div>");
            sbHtml.AppendLine("        </div>");
            sbHtml.AppendLine("    </div>");
            sbHtml.AppendLine("    <script>");
            sbHtml.AppendLine("        $(document).ready(function () {");
            sbHtml.Append(sbJS.ToString());
            sbHtml.AppendLine("        });");
            sbHtml.AppendLine("    </script>");

            return sbHtml.ToString();
        }
    }
}