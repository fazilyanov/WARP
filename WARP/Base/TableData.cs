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
            Dictionary<string, string> filterList = (Dictionary<string, string>)HttpContext.Current.Session[BaseSql + TableSql + PageName + "UserFilterList"];
            string id = string.Empty;
            string text = string.Empty;
            string idCond = string.Empty;
            string textCond = string.Empty;

            string key = string.Empty;

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

            // Очистка AC
            sbFunc.AppendLine("     function ClearAC(name) {");
            sbFunc.AppendLine("         $('#Id'+name).val('0');");
            sbFunc.AppendLine("         $('#'+name).val('');");
            sbFunc.AppendLine("     }");

            // Очистка Условия
            sbFunc.AppendLine("     function ClearCond(name) {");
            sbFunc.AppendLine("         $('#Id'+name+'Cond').val('0');");
            sbFunc.AppendLine("         $('#'+name+'Cond').val('');");
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
                        id = "0";
                        text = string.Empty;
                        idCond = "0";
                        textCond = string.Empty;

                        if (filterList != null)
                        {
                            key = "Id" + item.NameSql;
                            id = filterList.ContainsKey(key) ? filterList[key] : "0";
                            key = item.NameSql;
                            text = (id != "0" && filterList.ContainsKey(key)) ? filterList[key] : "";

                            key = "Id" + item.NameSql + "Cond";
                            idCond = filterList.ContainsKey(key) ? filterList[key] : "0";
                            key = item.NameSql + "Cond";
                            textCond = (idCond != "0" && filterList.ContainsKey(key)) ? filterList[key] : "";
                        }

                        sbResult.AppendLine("                    <div class=\"row\">");

                        sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                        sbResult.AppendLine("                            <div class=\"filter-field\">" + (string.IsNullOrEmpty(item.CaptionFilter) ? item.Caption : item.CaptionFilter) + "</div>");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.NameSql + "Cond\" name=\"" + item.NameSql + "Cond\" onchange=\"if ($('#" + item.NameSql + "Cond').val().trim() == '')$('#Id" + item.NameSql + "Cond').val('0');\" ");
                        sbResult.AppendLine("                                       value=\"\" class=\"form-control input-sm\" placeholder=\"Равно\" value=\"" + textCond + "\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" type=\"button\" onclick=\"ClearCond('" + item.NameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           <input type=\"hidden\" id=\"Id" + item.NameSql + "Cond\" name=\"Id" + item.NameSql + "Cond\" value=\"" + idCond + "\">");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                        <div class=\"col-sm-6\">");
                        sbResult.AppendLine("                           <div id=\"scrollable-dropdown-menu\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.NameSql + "\" name=\"" + item.NameSql + "\" onchange=\"if ($('#" + item.NameSql + "').val().trim() == '')$('#Id" + item.NameSql + "').val(0);\" ");
                        sbResult.AppendLine("                                       class=\"form-control input-sm\"  value=\"" + text + "\" placeholder=\"Начните вводить для поиска..\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" type=\"button\"><span class=\"glyphicon glyphicon-option-horizontal\"></span></button>");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" type=\"button\" onclick=\"ClearAC('" + item.NameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           </div>");
                        sbResult.AppendLine("                           <input type=\"hidden\" id=\"Id" + item.NameSql + "\" name=\"Id" + item.NameSql + "\" value=\"" + id + "\">");
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