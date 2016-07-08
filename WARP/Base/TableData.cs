using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace WARP
{
    public enum TableColumnAlign
    {
        Left,
        Center,
        Right,
    }

    public enum TableColumnEditType
    {
        None,
        CurrentUser,
        CurrentDateTime,
        String,
        Autocomplete,
        Integer,
        Money,
        DropDown,
    }

    // TODO : Показывать установленный фильтр
    public enum TableColumnFilterType
    {
        None,
        String,
        Autocomplete,
        Integer,
        Money,
        DropDown,
    }

    public enum TableColumnType
    {
        String,
        Integer,
        Money,
        DateTime,
        Date,
    }

    public class TableColumn
    {
        public TableColumnAlign Align { get; set; } = TableColumnAlign.Left;
        public string Caption { get; set; } = string.Empty;
        public string CaptionFilter { get; set; } = string.Empty;
        public string CaptionShort { get; set; } = string.Empty;
        public int EditMax { get; set; } = -1;
        public int EditMin { get; set; } = -1;
        public bool EditRequired { get; set; } = false;
        public TableColumnEditType EditType { get; set; } = TableColumnEditType.None;
        public string FilterDefaultValue { get; set; } = string.Empty;
        public TableColumnFilterType FilterType { get; set; } = TableColumnFilterType.None;
        public string LookUpField { get; set; } = string.Empty;
        public string LookUpTable { get; set; } = string.Empty;
        public string NameSql { get; set; } = string.Empty;
        public TableColumnType Type { get; set; } = TableColumnType.String;
        public int Width { get; set; } = 100;
    }

    public class TableData
    {
        public string BaseSql { get; set; } = string.Empty;
        public List<TableColumn> ColumnList { get; set; } = null;
        public string PageName { get; set; } = string.Empty;
        public string TableSql { get; set; } = string.Empty;

        public string GenerateFilterFormDialog()
        {
            Dictionary<string, string> filterList = (Dictionary<string, string>)HttpContext.Current.Session[BaseSql + TableSql + PageName + "UserFilterList"];
            string id = string.Empty;
            string text = string.Empty;
            string idCond = string.Empty;
            string textCond = string.Empty;

            string stringCondSelector = string.Empty;

            string key = string.Empty;

            StringBuilder sbResult = new StringBuilder();
            StringBuilder sbJS = new StringBuilder();
            StringBuilder sbFunc = new StringBuilder();

            // Общие функции для элементов формы
            // Отправка формы
            sbFunc.AppendLine("        function FormSend() {");
            sbFunc.AppendLine("             var msg   = $('#filterform').serialize();");
            sbFunc.AppendLine("             $.ajax({");
            sbFunc.AppendLine("                 type: 'POST',");
            sbFunc.AppendLine("                 url: '/Handler/SessionHandler.ashx',");
            sbFunc.AppendLine("                 data: msg,");
            sbFunc.AppendLine("                 success: function(data) {");
            sbFunc.AppendLine("                     $('#modalFilterForm').modal('toggle');");
            sbFunc.AppendLine("                     $('#table_id').DataTable().draw();");
            sbFunc.AppendLine("                 },");
            sbFunc.AppendLine("             });");
            sbFunc.AppendLine("         }");
            sbFunc.AppendLine();

            // Очистка AC
            sbFunc.AppendLine("         function ClearAC(name) {");
            sbFunc.AppendLine("             $('#Id'+name).val('0');");
            sbFunc.AppendLine("             $('#'+name).val('');");
            sbFunc.AppendLine("         }");
            sbFunc.AppendLine();

            // Очистка Условия
            sbFunc.AppendLine("         function ClearCond(name) {");
            sbFunc.AppendLine("             $('#Id'+name+'Cond').val('0');");
            sbFunc.AppendLine("             $('#'+name+'Cond').val('');");
            sbFunc.AppendLine("         }");
            sbFunc.AppendLine();

            // Сбросить все
            sbFunc.AppendLine("         function ClearAll() {");
            sbFunc.AppendLine("             $('#act').val('clearfilter');");
            sbFunc.AppendLine("             $(\"button[id^=\'clear\']\").click();");
            sbFunc.AppendLine("             FormSend();");
            sbFunc.AppendLine("         }");
            sbFunc.AppendLine();

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
            sbResult.AppendLine("                   <input type=\"hidden\" id=\"page\" name=\"page\" value=\"" + BaseSql + TableSql + PageName + "\">");
            sbResult.AppendLine("                   <input type=\"hidden\" id=\"act\" name=\"act\" value=\"none\">");

            foreach (TableColumn item in ColumnList)
            {
                if (item.FilterType != TableColumnFilterType.None)
                {
                    sbResult.AppendLine("                    <div class=\"row\">");
                    sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                    sbResult.AppendLine("                            <div class=\"filter-field\">" + (string.IsNullOrEmpty(item.CaptionFilter) ? item.Caption : item.CaptionFilter) + "</div>");
                    sbResult.AppendLine("                        </div>");
                }
                switch (item.FilterType)
                {
                    case TableColumnFilterType.String:
                        stringCondSelector += (stringCondSelector.Length > 0 ? "," : "") + "#" + item.NameSql + "Cond";

                        text = string.Empty;
                        idCond = "0";
                        textCond = string.Empty;

                        if (filterList != null)
                        {
                            key = item.NameSql;
                            text = (filterList.ContainsKey(key)) ? filterList[key] : "";

                            key = "Id" + item.NameSql + "Cond";
                            idCond = filterList.ContainsKey(key) ? filterList[key] : "0";
                            key = item.NameSql + "Cond";
                            textCond = (idCond != "0" && filterList.ContainsKey(key)) ? filterList[key] : "";
                        }

                        sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.NameSql + "Cond\" name=\"" + item.NameSql + "Cond\" onchange=\"if ($('#" + item.NameSql + "Cond').val().trim() == '')$('#Id" + item.NameSql + "Cond').val('0');\" ");
                        sbResult.AppendLine("                                       value=\"\" class=\"form-control input-sm filter-input\" placeholder=\"Содержит\" value=\"" + textCond + "\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button id=\"clearcond" + item.NameSql + "\" class=\"btn btn-default btn-sm\" type=\"button\" onclick=\"ClearCond('" + item.NameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           <input type=\"hidden\" id=\"Id" + item.NameSql + "Cond\" name=\"Id" + item.NameSql + "Cond\" value=\"" + idCond + "\">");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                        <div class=\"col-sm-6\">");
                        sbResult.AppendLine("                           <div id=\"scrollable-dropdown-menu\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.NameSql + "\" name=\"" + item.NameSql + "\" onchange=\"if ($('#" + item.NameSql + "').val().trim() == '')$('#Id" + item.NameSql + "').val(0);\" ");
                        sbResult.AppendLine("                                       class=\"form-control input-sm filter-input\"  value=\"" + text + "\" placeholder=\"Текст для поиска\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" type=\"button\"><span class=\"glyphicon glyphicon-option-horizontal\"></span></button>");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" id=\"clear" + item.NameSql + "\" type=\"button\" onclick=\"ClearAC('" + item.NameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           </div>");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                    </div>");
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

                        sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.NameSql + "Cond\" name=\"" + item.NameSql + "Cond\" onchange=\"if ($('#" + item.NameSql + "Cond').val().trim() == '')$('#Id" + item.NameSql + "Cond').val('0');\" ");
                        sbResult.AppendLine("                                       value=\"\" class=\"form-control input-sm filter-input\" placeholder=\"Равно\" value=\"" + textCond + "\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button id=\"clearcond" + item.NameSql + "\" class=\"btn btn-default btn-sm\" type=\"button\" onclick=\"ClearCond('" + item.NameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           <input type=\"hidden\" id=\"Id" + item.NameSql + "Cond\" name=\"Id" + item.NameSql + "Cond\" value=\"" + idCond + "\">");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                        <div class=\"col-sm-6\">");
                        sbResult.AppendLine("                           <div id=\"scrollable-dropdown-menu\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.NameSql + "\" name=\"" + item.NameSql + "\" onchange=\"if ($('#" + item.NameSql + "').val().trim() == '')$('#Id" + item.NameSql + "').val(0);\" ");
                        sbResult.AppendLine("                                       class=\"form-control input-sm filter-input\"  value=\"" + text + "\" placeholder=\"Начните вводить для поиска по справочнику..\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" type=\"button\"><span class=\"glyphicon glyphicon-option-horizontal\"></span></button>");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" id=\"clear" + item.NameSql + "\" type=\"button\" onclick=\"ClearAC('" + item.NameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
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
                        sbJS.AppendLine();

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
                        sbJS.AppendLine();
                        sbJS.AppendLine("            $(\"#" + item.NameSql + "\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#Id" + item.NameSql + "\").val(datum.ID); });");
                        sbJS.AppendLine();

                        break;

                    case TableColumnFilterType.Integer:
                        break;

                    case TableColumnFilterType.Money:
                        break;

                    default:
                        break;
                }
            }

            // Если есть поиск по стороковым полям
            if (stringCondSelector.Length > 0)
            {
                sbJS.AppendLine("            // Для строковых условий: ");
                sbJS.AppendLine("            var sourceStringCond = new Bloodhound({");
                sbJS.AppendLine("                datumTokenizer: Bloodhound.tokenizers.whitespace,");
                sbJS.AppendLine("                queryTokenizer: Bloodhound.tokenizers.whitespace,");
                sbJS.AppendLine("                remote: {");
                sbJS.AppendLine("                    url: '/Handler/sourceStringCond.json',");
                sbJS.AppendLine("                },");
                sbJS.AppendLine("                limit: 30,");
                sbJS.AppendLine("            });");
                sbJS.AppendLine();

                sbJS.AppendLine("            $('" + stringCondSelector + "').typeahead({");
                sbJS.AppendLine("                highlight: true,");
                sbJS.AppendLine("                minLength: 0,");
                sbJS.AppendLine("            },");
                sbJS.AppendLine("            {");
                sbJS.AppendLine("                name: 'thStringCond',");
                sbJS.AppendLine("                display: 'Name',");
                sbJS.AppendLine("                highlight: true,");
                sbJS.AppendLine("                limit: 30,");
                sbJS.AppendLine("                source: sourceStringCond,");
                sbJS.AppendLine("            });");
                sbJS.AppendLine();
                foreach (string item in stringCondSelector.Split(','))
                {
                    sbJS.AppendLine("            $(\"" + item + "\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#Id" + item.Substring(1) + "\").val(datum.ID); });");
                }
                sbJS.AppendLine();
            }

            sbResult.AppendLine("                </form>");
            sbResult.AppendLine("                </div>");
            sbResult.AppendLine("                <div class=\"modal-footer\">");
            sbResult.AppendLine("                   <div style=\"float: left;\">");
            sbResult.AppendLine("                       <button type=\"button\" class=\"btn btn-default\" onclick=\"ClearAll()\">Сбросить все</button>");
            sbResult.AppendLine("                   </div>");
            sbResult.AppendLine("                   <div>");
            sbResult.AppendLine("                       <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Закрыть</button>");
            sbResult.AppendLine("                       <button type=\"button\" class=\"btn btn-primary\" onclick=\"$('#act').val('setfilter'); FormSend()\">Применить</button>");
            sbResult.AppendLine("                   </div>");
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

        public string GenerateHtmlTableColumns()
        {
            string ret = Environment.NewLine;
            foreach (TableColumn item in ColumnList)
            {
                ret += "               <th>" + item.Caption + "</th>" + Environment.NewLine;
            }
            return ret;
        }

        // Генерит список js полей для editor'а
        public string GenerateJSEditorTableColumns()
        {
            string ret = Environment.NewLine;
            foreach (TableColumn item in ColumnList)
            {
                switch (item.EditType)
                {
                    case TableColumnEditType.String:
                        ret += "                    { label: \"" + item.Caption + ":\", name: \"" + item.NameSql + "\" }," + Environment.NewLine;
                        break;

                    case TableColumnEditType.Autocomplete:
                    case TableColumnEditType.Integer:
                    case TableColumnEditType.Money:
                    case TableColumnEditType.DropDown:
                        break;

                    case TableColumnEditType.None:
                    case TableColumnEditType.CurrentUser:
                    case TableColumnEditType.CurrentDateTime:
                        break;
                }
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

        public string GenerateWhereClause()
        {
            StringBuilder sbWhere = new StringBuilder();
            Dictionary<string, string> filterList = (Dictionary<string, string>)HttpContext.Current.Session[BaseSql + TableSql + PageName + "UserFilterList"];
            if (filterList != null)
            {
                string key = string.Empty;
                string value = string.Empty;
                foreach (TableColumn item in ColumnList)
                {
                    switch (item.FilterType)
                    {
                        case TableColumnFilterType.String:
                            key = item.NameSql;
                            if (filterList.ContainsKey(key))
                            {
                                value = filterList[key];
                                string buf = "    AND a.[" + item.NameSql + "]";
                                key = "Id" + item.NameSql + "Cond";
                                if (filterList.ContainsKey(key))
                                {
                                    switch (filterList[key])
                                    {
                                        case "1":
                                            buf += " = '" + value + "'";
                                            break;

                                        case "2":
                                            buf += " LIKE '" + value + "%'";
                                            break;

                                        case "3":
                                            buf += " LIKE '%" + value + "'";
                                            break;

                                        default:
                                            buf += " LIKE '%" + value + "%'";
                                            break;
                                    }
                                }
                                else
                                {
                                    buf += " LIKE '%" + value + "%'";
                                }
                                sbWhere.AppendLine(buf);
                            }
                            break;

                        case TableColumnFilterType.DropDown:
                        case TableColumnFilterType.Autocomplete:
                            key = "Id" + item.NameSql;
                            if (filterList.ContainsKey(key))
                            {
                                sbWhere.AppendLine("    AND a.[" + key + "] = " + filterList[key]);
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

        // TODO :  Разобраться
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

        internal string Check(string curBase, string curTable, string curPage, string action, Dictionary<string, List<RequestData>> rows)
        {
            List<FieldErrors> fieldErrors = new List<FieldErrors>();

            // Проверяем каждую пару, для первой или единственной строки
            foreach (RequestData rd in rows.First().Value)
            {
                // Флаг для прерывания проверки или продолжения проверки
                bool resume = true;

                // Ищем настройки для этого поля, по переданному имени поля
                TableColumn tableColumn = ColumnList.Find(x => x.NameSql == rd.FieldName);

                // Проверяем тип введенных данных

                // Обязательность заполнения поля
                if (resume && tableColumn.EditRequired && string.IsNullOrEmpty(rd.FieldValue))
                {
                    fieldErrors.Add(new FieldErrors { name = tableColumn.NameSql, status = "Поле обязательно для заполнения" });
                    resume = false;
                }

                // Ограничения
                if (resume)
                {
                    switch (tableColumn.EditType)
                    {
                        case TableColumnEditType.CurrentDateTime:
                            break;

                        case TableColumnEditType.String:
                            if (tableColumn.EditMax > -1 && rd.FieldValue.Length > tableColumn.EditMax)
                            {
                                fieldErrors.Add(new FieldErrors { name = tableColumn.NameSql, status = "Максимально допустимая длина поля: " + tableColumn.EditMax + " симв." });
                                resume = false;
                            }

                            if (tableColumn.EditMin > -1 && rd.FieldValue.Length < tableColumn.EditMin)
                            {
                                fieldErrors.Add(new FieldErrors { name = tableColumn.NameSql, status = "Минимально допустимая длина поля: " + tableColumn.EditMax + " симв." });
                                resume = false;
                            }
                            break;

                        case TableColumnEditType.Integer:
                        case TableColumnEditType.Money:
                            // TODO :
                            break;
                    }
                }
            }
        }//{"fieldErrors":[{"name":"first_name","status":"This field is required"},{"name":"last_name","status":"This field is required"}],"data":[]}

        public string Save(string curBase, string curTable, string curPage, string action, Dictionary<string, List<RequestData>> rows)
        {
            string result = string.Empty;

            // Запрос
            StringBuilder query = new StringBuilder();

            // Список параметров
            List<SqlParameter> param = new List<SqlParameter>();

            // Открываем подключение, начинаем общую транзакцию
            SqlConnection sqlConnection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            sqlConnection.Open();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
            SqlCommand sqlCommand = new SqlCommand(query.ToString(), sqlConnection, sqlTransaction);

            try
            {
                // Выбираем переданное действие
                switch (action)
                {
                    case "create":

                        break;

                    case "edit":

                        // Для каждой переданной строки с данными, создаем строку запроса и параметры к ней, выполняем запрос
                        foreach (KeyValuePair<string, List<RequestData>> pair in rows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            // Копируем текущую строку в таблицу истрории
                            query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "History] Select * from[dbo].[" + curBase + curTable + "] where ID = @ID;");

                            // Обновляем запись в главной таблице
                            query.AppendLine("UPDATE[dbo].[" + curBase + curTable + "] SET");
                            query.AppendLine("     [IdUser] = @IdUser"); // Пользователь внесший изменения
                            query.AppendLine("    ,[DateUpd] = GetDate()"); // Дата внесения
                            foreach (RequestData rd in pair.Value)
                            {
                                query.AppendLine("    ,[" + rd.FieldName + "] = @" + rd.FieldName);
                                param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.NVarChar, Value = rd.FieldValue });
                            }
                            query.AppendLine("WHERE ID = @ID");

                            param.Add(new SqlParameter { ParameterName = "@ID", SqlDbType = SqlDbType.Int, Value = pair.Key });
                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = HttpContext.Current.Session["UserId"].ToString() });

                            sqlCommand = new SqlCommand(query.ToString(), sqlConnection, sqlTransaction);
                            sqlCommand.Parameters.AddRange(param.ToArray());
                            sqlCommand.ExecuteNonQuery();
                        }

                        break;

                    case "remove":
                        break;
                }

                // Если ошибок не было коммитим
                sqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                result = "Ошибка при сохранении: " + ex.Message.Trim();
                sqlTransaction.Rollback();
                ComFunc.LogSqlError(ex.Message.Trim(), sqlCommand.CommandText, param.ToArray());
            }
            finally
            {
                sqlConnection.Close();
            }

            return result;
        }
    }
}