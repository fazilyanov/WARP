using System;
using System.Collections.Generic;
using System.Text;
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

    public enum TableColumnFilterType
    {
        None,
        String,
        Autocomplete,
        Integer,
        Money,
        DropDown,
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
        public string BaseSql { get; set; } = string.Empty;
        public string TableSql { get; set; } = string.Empty;
        public string PageName { get; set; } = string.Empty;

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
            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbJS = new StringBuilder();
            StringBuilder sbFunc = new StringBuilder();

            // Отправка формы
            sbFunc.AppendLine("     function FormSend() {");
            sbFunc.AppendLine("         var msg   = $('#filterform').serialize();");
            sbFunc.AppendLine("         $.ajax({");
            sbFunc.AppendLine("             type: 'POST',");
            sbFunc.AppendLine("             url: '/Handler/SessionHandler.ashx',");
            sbFunc.AppendLine("             data: msg,");
            sbFunc.AppendLine("             success: function(data) {");
            sbFunc.AppendLine("                 $('#table_id').DataTable().draw();");
            sbFunc.AppendLine("                 $('#modalFilterForm').modal('toggle');");
            sbFunc.AppendLine("             },");
            sbFunc.AppendLine("         });");
            sbFunc.AppendLine("     }");

            // Форма
            sbResult.AppendLine("    <div class=\"modal fade\" id=\"modalFilterForm\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"modalFilterForm\" aria-hidden=\"true\">");
            sbResult.AppendLine("        <div class=\"modal-dialog modal-lg\">");
            sbResult.AppendLine("            <div class=\"modal-content\">");
            sbResult.AppendLine("                <div class=\"modal-header\">");
            sbResult.AppendLine("                    <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\">&times;</button>");
            sbResult.AppendLine("                    <h4 class=\"modal-title\">Установить фильтр</h4>");
            sbResult.AppendLine("                </div>");
            sbResult.AppendLine("                <div class=\"modal-body\">");
            sbResult.AppendLine("                <form name=\"filterform\" method=\"POST\" id=\"filterform\" action=\"javascript: void(null);\">");
            sbResult.AppendLine("                   <input type=\"hidden\" name=\"page\" value=\"" + BaseSql + TableSql + PageName + "\">");
            foreach (TableColumn item in ColumnList)
            {
                switch (item.FilterType)
                {
                    case TableColumnFilterType.String:
                        break;

                    case TableColumnFilterType.DropDown:
                    case TableColumnFilterType.Autocomplete:
                        sbResult.AppendLine("                    <div class=\"row\">");
                        sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                        sbResult.AppendLine("                            <h5>" + (string.IsNullOrEmpty(item.CaptionFilter) ? item.Caption : item.CaptionFilter) + "</h5>");
                        sbResult.AppendLine("                        </div>");
                        sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                        sbResult.AppendLine("                             <select name=\"cond" + item.NameSql + "\"  id=\"cond" + item.NameSql + "\" class=\"combobox form-control input-sm\">");
                        sbResult.AppendLine("                                 <option></option>");
                        sbResult.AppendLine("                                 <option value=\"=\" selected>Равно</option>");
                        sbResult.AppendLine("                             </select>");
                        sbResult.AppendLine("                        </div>");
                        sbResult.AppendLine("                        <div class=\"col-sm-6\">");
                        sbResult.AppendLine("                           <div id=\"scrollable-dropdown-menu\">");
                        sbResult.AppendLine("                               <input type=\"text\"  id=\"" + item.NameSql + "\" name=\"" + item.NameSql + "\" class=\"form-control input-sm\" placeholder=\"Начните вводить для поиска..\">");
                        sbResult.AppendLine("                               <input type=\"hidden\" id=\"Id" + item.NameSql + "\" name=\"Id" + item.NameSql + "\">");
                        sbResult.AppendLine("                           </div>");
                        sbResult.AppendLine("                        </div>");
                        sbResult.AppendLine("                    </div>");

                        sbJS.AppendLine();
                        sbJS.AppendLine("            // Для столбца: " + item.Caption);
                        sbJS.AppendLine("            var source" + item.NameSql + " = new Bloodhound({");
                        sbJS.AppendLine("                datumTokenizer: Bloodhound.tokenizers.whitespace,");
                        sbJS.AppendLine("                queryTokenizer: Bloodhound.tokenizers.whitespace,");
                        sbJS.AppendLine("                remote: {");
                        sbJS.AppendLine("                    url: '/Handler/TypeaheadHandler.ashx?t=" + item.LookUpTable + "&q=%QUERY',");
                        sbJS.AppendLine("                    wildcard: '%QUERY'");
                        sbJS.AppendLine("                },");
                        sbJS.AppendLine("                limit: 30,");
                        sbJS.AppendLine("            });");

                        sbJS.AppendLine("            $('#scrollable-dropdown-menu #" + item.NameSql + "').typeahead({");
                        sbJS.AppendLine("                highlight: true,");
                        sbJS.AppendLine("                minLength: " + (item.FilterType == TableColumnFilterType.DropDown ? "0" : "1") + ",");
                        sbJS.AppendLine("            },");
                        sbJS.AppendLine("            {");
                        
                        sbJS.AppendLine("                name: 'th" + item.NameSql + "',");
                        sbJS.AppendLine("                display: 'Name',");
                        sbJS.AppendLine("                highlight: true,");
                        sbJS.AppendLine("                limit: 30,");
                        sbJS.AppendLine("                source: source" + item.NameSql + ",");
                        sbJS.AppendLine("            });");

                        sbJS.AppendLine("            $(\"#" + item.NameSql + "\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#Id" + item.NameSql + "\").val(datum.ID); });");
                        sbJS.AppendLine("            $('#cond" + item.NameSql + "').combobox();");
                        break;

                    case TableColumnFilterType.Integer:
                        break;

                    case TableColumnFilterType.Money:
                        break;

                    default:
                        break;
                }
            }

            sbResult.AppendLine("                </form>");
            sbResult.AppendLine("                </div>");
            sbResult.AppendLine("                <div class=\"modal-footer\">");
            sbResult.AppendLine("                    <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Закрыть</button>");
            sbResult.AppendLine("                    <button type=\"button\" class=\"btn btn-primary\" onclick=\"FormSend()\">Применить</button>");
            sbResult.AppendLine("                </div>");
            sbResult.AppendLine("            </div>");
            sbResult.AppendLine("        </div>");
            sbResult.AppendLine("    </div>");
            sbResult.AppendLine("    <script>");
            sbResult.Append(sbFunc.ToString());
            sbResult.AppendLine("        $(document).ready(function () {");
            sbResult.Append(sbJS.ToString());
            sbResult.AppendLine("        });");
            sbResult.AppendLine("    </script>");

            return sbResult.ToString();
        }

        public string GenerateWhereClause()
        {
            StringBuilder sbWhere = new StringBuilder();
            Dictionary<string, string> filterList = (Dictionary<string, string>)HttpContext.Current.Session[BaseSql + TableSql + PageName + "UserFilterList"];
            if (filterList != null)
            {
                string key = string.Empty;
                foreach (TableColumn item in ColumnList)
                {
                    switch (item.FilterType)
                    {
                        case TableColumnFilterType.String:
                            break;

                        case TableColumnFilterType.DropDown:
                        case TableColumnFilterType.Autocomplete:
                            key = "Id" + item.NameSql;
                            if (filterList.ContainsKey(key))
                            {
                                sbWhere.AppendLine("    AND a.[Id" + item.NameSql + "] = " + filterList["Id" + item.NameSql]);
                            }
                            break;

                        case TableColumnFilterType.Integer:
                            break;

                        case TableColumnFilterType.Money:
                            break;

                        default:
                            break;
                    }
                }
            }
            return sbWhere.ToString();
        }
    }
}