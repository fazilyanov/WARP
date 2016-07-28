using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace WARP
{
    public class TableData
    {
        #region Свойства

        // Список строк переданных для редактирования
        public Dictionary<string, List<RequestData>> RequestRows { get; set; } = null;

        // Тип операции переданный гридом при редактировании
        public TableAction Action { get; set; } = TableAction.None;

        // База | Организация
        public string SqlBase { get; set; } = string.Empty;

        // Список полей
        public List<TableColumn> ColumnList { get; set; } = null;

        // Количество строк, которые нужно показать
        public int DisplayLength { get; set; } = 500;

        // Количество строк, которые нужно пропустить
        public int DisplayStart { get; set; } = 0;

        // Счетчик запросов от грида, нужно его возвращать неизменным
        public int DrawCount { get; set; } = 0;

        // Страница
        public string PageName { get; set; } = string.Empty;

        // Текст в шапке грида
        public string PageTitle { get; set; } = string.Empty;

        // Столбец для сортировки
        public string SortCol { get; set; } = "ID";

        // Направление сортировки
        public TableSortDir SortDir { get; set; } = TableSortDir.Desc;

        // Таблица
        public string TableSql { get; set; } = string.Empty;

        // Показывать записи с пометкой на удаление
        public bool ShowDelRows { get; set; } = false;

        // Показывать предыдущие версии
        public bool ShowNoneActiveRows { get; set; } = false;

        // Показывать инфо "плюсик" для записи
        public bool ShowRowInfoButtom { get; set; } = false;
        

        #endregion Свойства

        #region Инициализация

        // Конструктор
        public TableData()
        {
        }

        // Инициализация
        public void Init(string baseSql, string tableSql)
        {
            SqlBase = baseSql;
            TableSql = tableSql;
        }

        // Инициализация
        public void Init(string baseSql, string tableSql, string pageName)
        {
            SqlBase = baseSql;
            TableSql = tableSql;
            PageName = pageName;
        }

        // Инициализация
        public void Init(string baseSql, string tableSql, string pageName, int drawCount, int displayStart, int displayLength, int sortCol, string sortDir)
        {
            SqlBase = baseSql;
            TableSql = tableSql;
            PageName = pageName;
            DrawCount = drawCount;
            DisplayStart = displayStart;
            DisplayLength = displayLength;
            // Если есть "плюсик" нумерация столбцов сбивается
            if (ShowRowInfoButtom && sortCol > 0) sortCol--;
            SortCol = ColumnList.Count >= sortCol ? ColumnList[sortCol].DataNameSql : string.Empty;
            SortDir = sortDir == "asc" ? TableSortDir.Asc : TableSortDir.Desc;
        }

        #endregion Инициализация

        #region Генерация HTML|JS

        // Форма для фильтра
        public string GenerateFilterFormDialog()
        {
            Dictionary<string, string> filterList = (Dictionary<string, string>)HttpContext.Current.Session[SqlBase + TableSql + PageName + "UserFilterList"];
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
            sbFunc.AppendLine();
            sbFunc.AppendLine("        function FormSend() {");
            sbFunc.AppendLine("             var msg   = $('#filterform').serialize();");
            sbFunc.AppendLine("             $.ajax({");
            sbFunc.AppendLine("                 type: 'POST',");
            sbFunc.AppendLine("                 url: '/Handler/SessionHandler.ashx',");
            sbFunc.AppendLine("                 data: msg,");
            sbFunc.AppendLine("                 success: function(data) {");
            sbFunc.AppendLine("                     $('#modalFilterForm').modal('toggle');");
            sbFunc.AppendLine("                     $('#table" + TableSql + "').DataTable().draw();");
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
            sbResult.AppendLine("    <div class=\"modal\" id=\"modalFilterForm\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"modalFilterForm\" aria-hidden=\"true\">");
            sbResult.AppendLine("        <div class=\"modal-dialog modal-lg\">");
            sbResult.AppendLine("            <div class=\"modal-content\">");
            sbResult.AppendLine("                <div class=\"modal-header\">");
            sbResult.AppendLine("                    <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\">&times;</button>");
            sbResult.AppendLine("                    <h4 class=\"modal-title\">Установить фильтр</h4>");
            sbResult.AppendLine("                </div>");
            sbResult.AppendLine("                <div class=\"modal-body\">");
            sbResult.AppendLine("                <form name=\"filterform\" method=\"POST\" id=\"filterform\" action=\"javascript: void(null);\">");
            sbResult.AppendLine("                   <input type=\"hidden\" id=\"page\" name=\"page\" value=\"" + SqlBase + TableSql + PageName + "\">");
            sbResult.AppendLine("                   <input type=\"hidden\" id=\"act\" name=\"act\" value=\"none\">");

            foreach (TableColumn item in ColumnList)
            {
                if (item.FilterType != TableColumnFilterType.None)
                {
                    sbResult.AppendLine("                    <div class=\"row\">");
                    sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                    sbResult.AppendLine("                            <div class=\"filter-field\">" + (string.IsNullOrEmpty(item.FilterCaption) ? item.ViewCaption : item.FilterCaption) + "</div>");
                    sbResult.AppendLine("                        </div>");
                }
                switch (item.FilterType)
                {
                    case TableColumnFilterType.String:
                        stringCondSelector += (stringCondSelector.Length > 0 ? "," : "") + "#" + item.DataNameSql + "Cond";

                        text = string.Empty;
                        idCond = "0";
                        textCond = string.Empty;

                        if (filterList != null)
                        {
                            key = item.DataNameSql;
                            text = (filterList.ContainsKey(key)) ? filterList[key] : "";

                            key = "Id" + item.DataNameSql + "Cond";
                            idCond = filterList.ContainsKey(key) ? filterList[key] : "0";
                            key = item.DataNameSql + "Cond";
                            textCond = (idCond != "0" && filterList.ContainsKey(key)) ? filterList[key] : "";
                        }

                        sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.DataNameSql + "Cond\" name=\"" + item.DataNameSql + "Cond\" onchange=\"if ($('#" + item.DataNameSql + "Cond').val().trim() == '')$('#Id" + item.DataNameSql + "Cond').val('0');\" ");
                        sbResult.AppendLine("                                       value=\"\" class=\"form-control input-sm filter-input\" placeholder=\"Содержит\" value=\"" + textCond + "\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button id=\"clearcond" + item.DataNameSql + "\" class=\"btn btn-default btn-sm\" type=\"button\" onclick=\"ClearCond('" + item.DataNameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           <input type=\"hidden\" id=\"Id" + item.DataNameSql + "Cond\" name=\"Id" + item.DataNameSql + "Cond\" value=\"" + idCond + "\">");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                        <div class=\"col-sm-6\">");
                        sbResult.AppendLine("                           <div id=\"scrollable-dropdown-menu\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.DataNameSql + "\" name=\"" + item.DataNameSql + "\" onchange=\"if ($('#" + item.DataNameSql + "').val().trim() == '')$('#Id" + item.DataNameSql + "').val(0);\" ");
                        sbResult.AppendLine("                                       class=\"form-control input-sm filter-input\"  value=\"" + text + "\" placeholder=\"Текст для поиска\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" type=\"button\"><span class=\"glyphicon glyphicon-option-horizontal\"></span></button>");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" id=\"clear" + item.DataNameSql + "\" type=\"button\" onclick=\"ClearAC('" + item.DataNameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
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
                            key = "Id" + item.DataNameSql;
                            id = filterList.ContainsKey(key) ? filterList[key] : "0";
                            key = item.DataNameSql;
                            text = (id != "0" && filterList.ContainsKey(key)) ? filterList[key] : "";

                            key = "Id" + item.DataNameSql + "Cond";
                            idCond = filterList.ContainsKey(key) ? filterList[key] : "0";
                            key = item.DataNameSql + "Cond";
                            textCond = (idCond != "0" && filterList.ContainsKey(key)) ? filterList[key] : "";
                        }

                        sbResult.AppendLine("                        <div class=\"col-sm-3\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.DataNameSql + "Cond\" name=\"" + item.DataNameSql + "Cond\" onchange=\"if ($('#" + item.DataNameSql + "Cond').val().trim() == '')$('#Id" + item.DataNameSql + "Cond').val('0');\" ");
                        sbResult.AppendLine("                                       value=\"\" class=\"form-control input-sm filter-input\" placeholder=\"Равно\" value=\"" + textCond + "\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button id=\"clearcond" + item.DataNameSql + "\" class=\"btn btn-default btn-sm\" type=\"button\" onclick=\"ClearCond('" + item.DataNameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           <input type=\"hidden\" id=\"Id" + item.DataNameSql + "Cond\" name=\"Id" + item.DataNameSql + "Cond\" value=\"" + idCond + "\">");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                        <div class=\"col-sm-6\">");
                        sbResult.AppendLine("                           <div id=\"scrollable-dropdown-menu\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.DataNameSql + "\" name=\"" + item.DataNameSql + "\" onchange=\"if ($('#" + item.DataNameSql + "').val().trim() == '')$('#Id" + item.DataNameSql + "').val(0);\" ");
                        sbResult.AppendLine("                                       class=\"form-control input-sm filter-input\"  value=\"" + text + "\" placeholder=\"Начните вводить для поиска по справочнику..\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" type=\"button\"><span class=\"glyphicon glyphicon-option-horizontal\"></span></button>");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" id=\"clear" + item.DataNameSql + "\" type=\"button\" onclick=\"ClearAC('" + item.DataNameSql + "')\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           </div>");
                        sbResult.AppendLine("                           <input type=\"hidden\" id=\"Id" + item.DataNameSql + "\" name=\"Id" + item.DataNameSql + "\" value=\"" + id + "\">");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                    </div>");

                        sbJS.AppendLine();
                        sbJS.AppendLine("            // Для столбца: " + item.ViewCaption);
                        sbJS.AppendLine("            var source" + item.DataNameSql + " = new Bloodhound({");
                        sbJS.AppendLine("                datumTokenizer: Bloodhound.tokenizers.whitespace,");
                        sbJS.AppendLine("                queryTokenizer: Bloodhound.tokenizers.whitespace,");
                        sbJS.AppendLine("                remote: {");
                        sbJS.AppendLine("                    url: '/Handler/TypeaheadHandler.ashx?t=" + item.DataLookUpTable + "&q=%QUERY',");
                        sbJS.AppendLine("                    wildcard: '%QUERY'");
                        sbJS.AppendLine("                },");
                        sbJS.AppendLine("                limit: 30,");
                        sbJS.AppendLine("            });");
                        sbJS.AppendLine();

                        sbJS.AppendLine("            $('#scrollable-dropdown-menu #" + item.DataNameSql + "').typeahead({");
                        sbJS.AppendLine("                highlight: true,");
                        sbJS.AppendLine("                minLength: " + (item.FilterType == TableColumnFilterType.DropDown ? "0" : "1") + ",");
                        sbJS.AppendLine("            },");
                        sbJS.AppendLine("            {");
                        sbJS.AppendLine("                name: 'th" + item.DataNameSql + "',");
                        sbJS.AppendLine("                display: 'Name',");
                        sbJS.AppendLine("                highlight: true,");
                        sbJS.AppendLine("                limit: 30,");
                        sbJS.AppendLine("                source: source" + item.DataNameSql + ",");
                        sbJS.AppendLine("            });");
                        sbJS.AppendLine();
                        sbJS.AppendLine("            $(\"#" + item.DataNameSql + "\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#Id" + item.DataNameSql + "\").val(datum.ID); });");
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
            sbResult.AppendLine();
            sbResult.AppendLine("    <script>");
            sbResult.AppendLine(sbFunc.ToString());
            sbResult.AppendLine("        $(document).ready(function () {");
            sbResult.Append(sbJS.ToString());
            sbResult.AppendLine("        });");
            sbResult.AppendLine("    </script>");

            return sbResult.ToString();
        }

        //// Собирает все вместе
        //public string GenerateHtml()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    // Таблица HTML
        //    sb.AppendLine(GenerateHtmlTable());

        //    // Фильтр
        //    sb.AppendLine(GenerateFilterFormDialog());

        //    //sb.AppendLine("    <script>");
        //    sb.AppendLine("        var editor;");
        //    sb.AppendLine(GenerateJSWindowsResize());
        //    sb.AppendLine("        $(document).ready(function () {");
        //    sb.AppendLine("            $('#curPageTitle').text('" + PageTitle + "');");
        //    sb.AppendLine(GenerateJSEditorInit());
        //    sb.AppendLine(GenerateJSDataTable());
        //    sb.AppendLine("        });");
        //    //sb.AppendLine("    </script>");

        //    return sb.ToString();
        //}

        // HTML Таблица
        public string GenerateHtmlTable()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<table id=\"table" + TableSql + "\" class=\"table table-striped table-bordered table-condensed\" style=\"table-layout: fixed; width: 100%\">");
            sb.AppendLine("        <thead>");
            sb.AppendLine("            <tr>");
            if (ShowRowInfoButtom)
                sb.AppendLine("               <th></th>");
            foreach (TableColumn item in ColumnList)
            {
                sb.AppendLine("               <th>" + item.ViewCaption + "</th>");
            }
            sb.AppendLine("            </tr>");
            sb.AppendLine("        </thead>");
            sb.AppendLine("    </table>");

            return sb.ToString();
        }

        // Grid
        public string GenerateJSDataTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("            var table = $('#table" + TableSql + "').DataTable({");
            sb.AppendLine("                dom: '<\"row top-toolbar\"<\"col-sm-4\"B><\"col-sm-4\"p><\"col-sm-4\"i>>Zrt',");
            sb.AppendLine("                rowId: 'Id',");
            sb.AppendLine("                processing: true,");
            sb.AppendLine("                serverSide: true,");
            sb.AppendLine("                ajax: \"/Handler/GetDataHandler.ashx?curBase=" + SqlBase + "&curTable=" + TableSql + "&curPage=" + PageName + "\",");
            sb.AppendLine("                columns: [");
            sb.AppendLine(GenerateJSTableColumns());
            sb.AppendLine("                ],");
            sb.AppendLine("                autoWidth: false,");
            sb.AppendLine("                select: true,");
            sb.AppendLine("                colReorder: {realtime: false},");
            sb.AppendLine("                colResize: {\"tableWidthFixed\": true},");
            sb.AppendLine("                stateSave: true,");
            sb.AppendLine("                scrollY: ($(window).height() - 125) + \"px\",");
            sb.AppendLine("                scrollX: true,");
            sb.AppendLine("                scrollCollapse: false,");
            sb.AppendLine("                lengthMenu: [" + GenerateJSTableLengthMenu() + "],");
            sb.AppendLine("                pagingType6: \"simple\",");
            sb.AppendLine("                buttons: [");
            sb.AppendLine(GenerateJSTableButtons());
            sb.AppendLine("                ],");
            sb.AppendLine("                language: {");
            sb.AppendLine("                    url: '/content/DataTables-1.10.12/js/Russian.json'");
            sb.AppendLine("                }");
            sb.AppendLine("            });");
            return sb.ToString();
        }

        // Editor
        public string GenerateJSEditorInit()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("            editor = new $.fn.dataTable.Editor({");
            sb.AppendLine("                ajax: \"/Handler/SaveDataHandler.ashx?curBase=" + SqlBase + "&curTable=" + TableSql + "&curPage=" + PageName + "\",");
            sb.AppendLine("                table: \"#table" + TableSql + "\",");
            sb.AppendLine("                idSrc: 'Id',");
            sb.AppendLine("                fields: [");
            sb.AppendLine(GenerateJSEditorTableColumns());
            sb.AppendLine("                ],");
            sb.AppendLine("                i18n: {");
            sb.AppendLine("                    create: {");
            sb.AppendLine("                        button: \"Новая запись\",");
            sb.AppendLine("                        title: \"Создание новой записи\",");
            sb.AppendLine("                        submit: \"Создать\"");
            sb.AppendLine("                    },");
            sb.AppendLine("                    edit: {");
            sb.AppendLine("                        button: \"Редактировать\",");
            sb.AppendLine("                        title: \"Редактирование записи\",");
            sb.AppendLine("                        submit: \"Сохранить\"");
            sb.AppendLine("                    },");
            sb.AppendLine("                    remove: {");
            sb.AppendLine("                        button: \"Удалить\",");
            sb.AppendLine("                        title: \"Удаление\",");
            sb.AppendLine("                        submit: \"Подтвердить удаление\",");
            sb.AppendLine("                        confirm: {");
            sb.AppendLine("                            _: \"Подтвердите удаление %d записей?\",");
            sb.AppendLine("                            1: \"Подтвердите удаление записи?\"");
            sb.AppendLine("                        }");
            sb.AppendLine("                    },");
            sb.AppendLine("                    error: {");
            sb.AppendLine("                        system: \"Произошла ошибка\"");
            sb.AppendLine("                    },");
            sb.AppendLine("                    multi: {");
            sb.AppendLine("                        title: \"Множественное редактирование\",");
            sb.AppendLine("                        restore: \"Отменить\"");
            sb.AppendLine("                    },");
            sb.AppendLine("                }");
            sb.AppendLine("            });");
            return sb.ToString();
        }

        // Список полей для editor'а
        public string GenerateJSEditorTableColumns()
        {
            StringBuilder sb = new StringBuilder().AppendLine();

            foreach (TableColumn column in ColumnList)
            {
                // Если поле редактируется
                if (column.EditType != TableColumnEditType.None)
                {
                    sb.AppendLine("                         { ");
                    sb.AppendLine("                             label: \"" + column.ViewCaption + ":\",");
                    sb.AppendLine("                             name: \"" + column.DataNameSql + "\",");

                    // Подсказка для поля при редактировании, выглядит уебищно, стили подкрутитьь надо
                    if (!string.IsNullOrEmpty(column.EditFieldInfo))
                    {
                        sb.AppendLine("                             fieldInfo: \"" + column.EditFieldInfo + "\",");
                    }

                    // Значение по умолчанию
                    if (!string.IsNullOrEmpty(column.EditDefaultText))
                    {
                        sb.AppendLine("                             def: function() { return '" + column.EditDefaultText + "'},");
                    }
                    sb.AppendLine("                         },");
                }
            }

            return sb.ToString();
            // fieldInfo: "Enter the appointment date using the options above",
            //def: function() {
            //    return new Date()
            //}
        }

        // Кнопоки грида
        public string GenerateJSTableButtons()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("                    { extend: 'create', editor: editor, className: 'btn-sm', key: \"l\", text: '<span class=\"glyphicon glyphicon-plus\" title=\"Создать новую запись\"></span>' },");
            sb.AppendLine("                    { extend: 'edit', editor: editor, className: 'btn-sm', key: \"h\", text: '<span class=\"glyphicon glyphicon-pencil\" title=\"Редактировать запись\"></span>' },");
            sb.AppendLine("                    {");
            sb.AppendLine("                        extend: \"selectedSingle\",");
            sb.AppendLine("                        className: 'btn-sm',");
            sb.AppendLine("                        text: '<span class=\"glyphicon glyphicon-duplicate\" title=\"Создать новую запись копированием текущей\"></span>',");
            sb.AppendLine("                        action: function (e, dt, node, config) {");
            sb.AppendLine("                            var values = editor.edit(");
            sb.AppendLine("                                    table.row({ selected: true }).index(),");
            sb.AppendLine("                                    false");
            sb.AppendLine("                                )");
            sb.AppendLine("                                .val();");
            sb.AppendLine("                            editor");
            sb.AppendLine("                                .create({");
            sb.AppendLine("                                    title: 'Создание копированием записи',");
            sb.AppendLine("                                    buttons: 'Создать'");
            sb.AppendLine("                                })");
            sb.AppendLine("                                .set(values);");
            sb.AppendLine("                        }");
            sb.AppendLine("                    },");
            sb.AppendLine("                    { extend: 'remove', editor: editor, className: 'btn-sm btn-space', key: \"e\", text: '<span class=\"glyphicon glyphicon-trash\" title=\"Удалить текущую запись\"></span>' },");
            sb.AppendLine("                    {");
            sb.AppendLine("                        extend: 'collection',");
            sb.AppendLine("                        text: 'Настройка таблицы',");
            sb.AppendLine("                        buttons: [");
            sb.AppendLine("                            {");
            sb.AppendLine("                                extend: 'colvis',");
            sb.AppendLine("                                text: 'Видимость столбцов',");
            sb.AppendLine("                                postfixButtons: ['colvisRestore']");
            sb.AppendLine("                            },");
            sb.AppendLine("                            {");
            sb.AppendLine("                                extend: 'pageLength',");
            sb.AppendLine("                                text: 'Записей на страницу'");
            sb.AppendLine("                            },");
            sb.AppendLine("                            {");
            sb.AppendLine("                                text: 'Сбросить все настройки',");
            sb.AppendLine("                                action: function (e, dt, node, config) {");
            sb.AppendLine("                                    dt.state.clear();");
            sb.AppendLine("                                    window.location.reload();");
            sb.AppendLine("                                }");
            sb.AppendLine("                            }");
            sb.AppendLine("                        ],");
            sb.AppendLine("                        className: \"btn-sm\",");
            sb.AppendLine("                    },");
            sb.AppendLine("                    {");
            sb.AppendLine("                        text: 'Фильтр',");
            sb.AppendLine("                        action: function (e, dt, node, config) {");
            sb.AppendLine("                            $('#modalFilterForm').modal();");
            sb.AppendLine("                        },");
            sb.AppendLine("                        key: \"a\",");
            sb.AppendLine("                        className: \"btn-sm\",");
            sb.AppendLine("                    }");
            return sb.ToString();
        }

        // Список полей для грида
        public string GenerateJSTableColumns()
        {
            StringBuilder sb = new StringBuilder();
            if (ShowRowInfoButtom)
                sb.AppendLine("                    {\"className\": 'details-control',\"orderable\": false,\"data\":null,\"defaultContent\": '', \"width\":\"20px\"},");
            foreach (TableColumn item in ColumnList)
                sb.AppendLine("                    { \"data\": \"" + item.DataNameSql + "\", className:\"dt-body-" + item.ViewAlign.ToString().ToLower() + "\", \"width\": \"" + item.ViewWidth + "px\" },");
            return sb.ToString();
        }

        // Список количества записей на странице
        public string GenerateJSTableLengthMenu()
        {
            return "[30, 100, 200, 500], ['30 строк', '100 строк', '200 строк', '500 строк']";
        }

        #endregion Генерация HTML|JS

        #region Получение данных

        public string GenerateWhereClause()
        {
            StringBuilder sbWhere = new StringBuilder();
            Dictionary<string, string> filterList = (Dictionary<string, string>)HttpContext.Current.Session[SqlBase + TableSql + PageName + "UserFilterList"];
            if (filterList != null)
            {
                string key = string.Empty;
                string value = string.Empty;
                foreach (TableColumn item in ColumnList)
                {
                    switch (item.FilterType)
                    {
                        case TableColumnFilterType.String:
                            key = item.DataNameSql;
                            if (filterList.ContainsKey(key))
                            {
                                value = filterList[key];
                                string buf = "    AND a.[" + item.DataNameSql + "]";
                                key = "Id" + item.DataNameSql + "Cond";
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
                            key = "Id" + item.DataNameSql;
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

        // Возвращает таблицу с данными для текущих настроек,
        // ids - список id для получения обновленных данных после массового редактирования
        public virtual DataTable GetData(string ids = null)
        {
            // Итоговый запрос
            StringBuilder sbQuery = new StringBuilder();

            // Условия отборки
            StringBuilder sbWhere = new StringBuilder();

            // Если нужно показать только удаленные
            if (!ShowDelRows)
                sbWhere.AppendLine("	a.Del=0 ");
            else
                sbWhere.AppendLine("	a.Del=1 ");

            // По умолчанию показываем только активные версии
            if (!ShowNoneActiveRows)
                sbWhere.AppendLine("	AND a.Active=1 ");

            // Пользовательский фильтр
            sbWhere.AppendLine(GenerateWhereClause());

            // Если передан список id
            if (!string.IsNullOrEmpty(ids))
                sbWhere.AppendLine("    AND a.Id in (" + ids + ")");

            // Считаем число записей, при таких условиях
            sbQuery.AppendLine("DECLARE @recordsFiltered int;");
            sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
            sbQuery.AppendLine("FROM [dbo].[" + SqlBase + TableSql + "] a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine(sbWhere.ToString() + ";");

            // Получаем данные
            sbQuery.AppendLine("SELECT * FROM  (");
            sbQuery.AppendLine("   SELECT @recordsFiltered AS recordsFiltered");
            sbQuery.AppendLine("   ,T.*");
            sbQuery.AppendLine("   ,U.Name as [User]");
            sbQuery.AppendLine("   FROM [dbo].[" + SqlBase + TableSql + "] T");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] U on T.IdUser = U.ID");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine(sbWhere.ToString());

            // Сортировка и пагинация
            sbQuery.AppendLine("ORDER BY a.[" + SortCol + "] " + SortDir);
            sbQuery.AppendLine("OFFSET @displayStart ROWS FETCH FIRST @displayLength ROWS ONLY");
            SqlParameter[] sqlParameterArray = {
                new SqlParameter { ParameterName = "@displayStart", SqlDbType = SqlDbType.Int, Value = DisplayStart },
                new SqlParameter { ParameterName = "@displayLength", SqlDbType = SqlDbType.Int, Value = DisplayLength }
            };

            // Выполняем запрос
            DataTable dt = ComFunc.GetData(sbQuery.ToString(), sqlParameterArray);
            return dt;
        }

        public string GetJsonData()
        {
            string ret = string.Empty;
            DataTable dt = GetData();
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            if (dt != null)
            {
                var result = new
                {
                    draw = DrawCount,
                    recordsTotal = (int)ComFunc.ExecuteScalar("SELECT COUNT(*) FROM [dbo].[" + SqlBase + TableSql + "] WHERE Del=0 AND Active=1"),
                    recordsFiltered = Convert.ToInt32(dt.Rows.Count > 0 ? dt.Rows[0]["recordsFiltered"] : 0),
                    data = GetFormatData(dt)
                };
                ret = javaScriptSerializer.Serialize(result);
            }
            else if (HttpContext.Current.Session["LastError"] != null)
            {
                ret = javaScriptSerializer.Serialize(new { error = HttpContext.Current.Session["LastError"].ToString() });
            }
            return ret;
        }

        private List<Dictionary<string, object>> GetFormatData(DataTable dt)
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            CultureInfo ruRu = CultureInfo.CreateSpecificCulture("ru-RU");
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();

                foreach (TableColumn column in ColumnList)
                {
                    switch (column.DataType)
                    {
                        case TableColumnType.Integer:
                            row.Add(column.DataNameSql, Convert.ToInt32(dr[column.DataNameSql]));
                            break;

                        case TableColumnType.Money:
                            row.Add(column.DataNameSql, String.Format(ruRu, "{0:0,0.00}", Convert.ToDecimal(dr[column.DataNameSql])));
                            break;

                        case TableColumnType.DateTime:
                            if (dr[column.DataNameSql] is DBNull)
                                row.Add(column.DataNameSql, string.Empty);
                            else
                                row.Add(column.DataNameSql, ((DateTime)dr[column.DataNameSql]).ToString("dd.MM.yyyy HH:mm:ss"));
                            break;

                        case TableColumnType.Date:
                            if (dr[column.DataNameSql] is DBNull)
                                row.Add(column.DataNameSql, string.Empty);
                            else
                                row.Add(column.DataNameSql, ((DateTime)dr[column.DataNameSql]).ToString("dd.MM.yyyy"));
                            break;

                        case TableColumnType.String:
                        default:
                            row.Add(column.DataNameSql, dr[column.DataNameSql].ToString());
                            break;
                    }
                }
                data.Add(row);
            }
            return data;
        }

        #endregion Получение данных

        #region Редактирование

        // Проверяем более специфические условия конкретной таблицы, можно перегружать
        public virtual string Check()
        {
            // TODO : проверка при удлении на использование
            return string.Empty;
        }

        // Обрабатывает запросы от грида/карточки на изменение данных
        public string Process()
        {
            // Ответ
            string result = string.Empty;

            // AJAX|JSON
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            // Чекаем на простые условия (обязательность, длинна и вообще изменилось ли что нибудь)
            if (Action != TableAction.Remove)
                result = Validate();

            // Чекаем на условия конкретной таблицы
            if (string.IsNullOrEmpty(result))
                result = Check();

            // Сохраняем
            if (string.IsNullOrEmpty(result))
                result = Save();

            // Если все прошло гладко, отправляем гриду обновленные данные
            if (string.IsNullOrEmpty(result))
            {
                // Список редактируемых ID
                string ids = string.Empty;
                foreach (string key in RequestRows.Keys) ids += key + ",";

                // Получаем обновленные данные по этим id из базы
                DataTable dt = GetData(ids.Substring(0, ids.Length - 1));

                // Возвращаем JSON
                if (dt != null)
                    result = javaScriptSerializer.Serialize(new { data = GetFormatData(dt) });
                else
                    result = javaScriptSerializer.Serialize(new { error = "Обновленные данные не получены" });
            }
            return result;
        }

        // Сохраняет изменения в базе
        public string Save()
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
                switch (Action)
                {
                    case TableAction.Create:
                        // Для каждой переданной строки с данными, создаем строку запроса и параметры к ней, выполняем запрос
                        foreach (KeyValuePair<string, List<RequestData>> pair in RequestRows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            // Обновляем запись в главной таблице
                            query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "]");
                            query.AppendLine("    ([IdUser]"); // Пользователь внесший изменения
                            query.AppendLine("    ,[DateUpd]"); // Дата внесения

                            foreach (RequestData rd in pair.Value)
                                query.AppendLine("    ,[" + rd.FieldName + "]");
                            query.AppendLine("    )");

                            query.AppendLine("VALUES ");
                            query.AppendLine("    (@IdUser");
                            query.AppendLine("    ,GetDate()");
                            param.Add(new SqlParameter { ParameterName = "@ID", SqlDbType = SqlDbType.Int, Value = pair.Key });
                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = HttpContext.Current.Session["UserId"].ToString() });
                            foreach (RequestData rd in pair.Value)
                            {
                                query.AppendLine("    ,@" + rd.FieldName);
                                param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.NVarChar, Value = rd.FieldValue });
                            }
                            query.AppendLine("    );");

                            sqlCommand = new SqlCommand(query.ToString(), sqlConnection, sqlTransaction);
                            sqlCommand.Parameters.AddRange(param.ToArray());
                            sqlCommand.ExecuteNonQuery();
                        }
                        break;

                    case TableAction.Edit:
                        // Для каждой переданной строки с данными, создаем строку запроса и параметры к ней, выполняем запрос
                        foreach (KeyValuePair<string, List<RequestData>> pair in RequestRows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            // Копируем текущую строку в таблицу истрории
                            query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "History] Select * from[dbo].[" + SqlBase + TableSql + "] where ID = @ID;");

                            // Обновляем запись в главной таблице
                            query.AppendLine("UPDATE[dbo].[" + SqlBase + TableSql + "] SET");
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

                    case TableAction.Remove:// TODO : удалять из основной и версий совсем устаревшие данные (полгода), routine
                        foreach (KeyValuePair<string, List<RequestData>> pair in RequestRows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            // Копируем текущую строку в таблицу истрории
                            query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "History] Select * from[dbo].[" + SqlBase + TableSql + "] where ID = @ID;");

                            // Обновляем запись в главной таблице
                            query.AppendLine("UPDATE[dbo].[" + SqlBase + TableSql + "] SET");
                            query.AppendLine("     [IdUser] = @IdUser"); // Пользователь внесший изменения
                            query.AppendLine("    ,[DateUpd] = GetDate()"); // Дата внесения
                            query.AppendLine("    ,[Del] = 1"); // Дата внесения
                            query.AppendLine("WHERE ID = @ID");

                            param.Add(new SqlParameter { ParameterName = "@ID", SqlDbType = SqlDbType.Int, Value = pair.Key });
                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = HttpContext.Current.Session["UserId"].ToString() });

                            sqlCommand = new SqlCommand(query.ToString(), sqlConnection, sqlTransaction);
                            sqlCommand.Parameters.AddRange(param.ToArray());
                            sqlCommand.ExecuteNonQuery();
                            result = "{}"; // После удаления, грид должен получить пустой объект
                        }
                        break;
                }

                // Если ошибок не было коммитим
                sqlTransaction.Commit();
            }
            catch (Exception ex)
            {
                sqlTransaction.Rollback();
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                result = javaScriptSerializer.Serialize(new { error = "Ошибка при сохранении: " + ex.Message.Trim() });

                ComFunc.LogSqlError(ex.Message.Trim(), sqlCommand.CommandText, param.ToArray());
            }
            finally
            {
                sqlConnection.Close();
            }

            return result;
        }

        // Проверяет простые условия (обязательность, длинна и тд)
        public string Validate()
        {
            // Флаг для прерывания/продолжения проверки
            bool resume = true;

            List<FieldErrors> fieldErrors = new List<FieldErrors>();
            // TODO : Проверять изменилась ли запись, если нет.. ничего не менять

            // Проверяем каждую пару, для первой или единственной строки
            if (resume)
                foreach (RequestData rd in RequestRows.First().Value)
                {
                    resume = true;

                    // Ищем настройки для этого поля, по переданному имени поля
                    TableColumn tableColumn = ColumnList.Find(x => x.DataNameSql == rd.FieldName);

                    // Обязательность заполнения поля
                    if (resume && tableColumn.EditRequired && string.IsNullOrEmpty(rd.FieldValue))
                    {
                        fieldErrors.Add(new FieldErrors { name = tableColumn.DataNameSql, status = "Поле обязательно для заполнения" });
                        resume = false;
                    }

                    // TODO :
                    // Проверяем тип введенных данных
                    //if (resume)
                    //{
                    //    switch (tableColumn.EditType)
                    //    {
                    //        case TableColumnEditType.Integer:
                    //            break;

                    //        case TableColumnEditType.Money:
                    //            break;

                    //        default:
                    //            break;
                    //    }

                    //    fieldErrors.Add(new FieldErrors { name = tableColumn.NameSql, status = "Неверный формат данных" });
                    //    resume = false;
                    //}

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
                                    fieldErrors.Add(new FieldErrors { name = tableColumn.DataNameSql, status = "Максимально допустимая длина поля: " + tableColumn.EditMax + " симв." });
                                    resume = false;
                                }

                                if (tableColumn.EditMin > -1 && rd.FieldValue.Length < tableColumn.EditMin)
                                {
                                    fieldErrors.Add(new FieldErrors { name = tableColumn.DataNameSql, status = "Минимально допустимая длина поля: " + tableColumn.EditMin + " симв." });
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

            // JSON
            if (fieldErrors.Count > 0)
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                return javaScriptSerializer.Serialize(new { fieldErrors = fieldErrors });
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion Редактирование
    }
}