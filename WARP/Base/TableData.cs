using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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

        // Список загруженных файлов
        public HttpFileCollection RequestFiles { get; set; } = null;

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

        #region Генерация HTML | JS

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
            // TODO: убрать
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
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.DataLookUpResult + "Cond\" name=\"" + item.DataLookUpResult + "Cond\" onchange=\"if ($('#" + item.DataLookUpResult + "Cond').val().trim() == '')$('#" + item.DataNameSql + "Cond').val('0');\" ");
                        sbResult.AppendLine("                                       value=\"\" class=\"form-control input-sm filter-input\" placeholder=\"Равно\" value=\"" + textCond + "\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button id=\"clearcond" + item.DataLookUpResult + "\" class=\"btn btn-default btn-sm\" type=\"button\" onclick=\"$('#" + item.DataNameSql + "Cond').val('0');$('#" + item.DataLookUpResult + "Cond').val('');\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           <input type=\"hidden\" id=\"" + item.DataNameSql + "Cond\" name=\"" + item.DataNameSql + "Cond\" value=\"" + idCond + "\">");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                        <div class=\"col-sm-6\">");
                        sbResult.AppendLine("                           <div id=\"scrollable-dropdown-menu\">");
                        sbResult.AppendLine("                               <div class=\"input-group\">");
                        sbResult.AppendLine("                                   <input type=\"text\"  id=\"" + item.DataLookUpResult + "\" name=\"" + item.DataLookUpResult + "\" onchange=\"if ($('#" + item.DataLookUpResult + "').val().trim() == '')$('#" + item.DataNameSql + "').val(0);\" ");
                        sbResult.AppendLine("                                       class=\"form-control input-sm filter-input\"  value=\"" + text + "\" placeholder=\"Начните вводить для поиска по справочнику..\">");
                        sbResult.AppendLine("                                   <span class=\"input-group-btn\">");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" type=\"button\"><span class=\"glyphicon glyphicon-option-horizontal\"></span></button>");
                        sbResult.AppendLine("                                       <button class=\"btn btn-default btn-sm\" id=\"clear" + item.DataLookUpResult + "\" type=\"button\" onclick=\"$('#" + item.DataNameSql + "').val('0');$('#" + item.DataLookUpResult + "').val('');\"><span class=\"glyphicon glyphicon-remove\"></span></button>");
                        sbResult.AppendLine("                                   </span>");
                        sbResult.AppendLine("                               </div>");
                        sbResult.AppendLine("                           </div>");
                        sbResult.AppendLine("                           <input type=\"hidden\" id=\"" + item.DataNameSql + "\" name=\"" + item.DataNameSql + "\" value=\"" + id + "\">");
                        sbResult.AppendLine("                        </div>");

                        sbResult.AppendLine("                    </div>");

                        sbJS.AppendLine();
                        sbJS.AppendLine("            // Для столбца: " + item.ViewCaption);
                        sbJS.AppendLine("            var source" + item.DataLookUpResult + " = new Bloodhound({");
                        sbJS.AppendLine("                datumTokenizer: Bloodhound.tokenizers.whitespace,");
                        sbJS.AppendLine("                queryTokenizer: Bloodhound.tokenizers.whitespace,");
                        sbJS.AppendLine("                remote: {");
                        sbJS.AppendLine("                    url: '/Handler/TypeaheadHandler.ashx?t=" + item.DataLookUpTable + "&q=%QUERY',");
                        sbJS.AppendLine("                    wildcard: '%QUERY'");
                        sbJS.AppendLine("                },");
                        sbJS.AppendLine("                limit: 30,");
                        sbJS.AppendLine("            });");
                        sbJS.AppendLine();

                        sbJS.AppendLine("            $('#scrollable-dropdown-menu #" + item.DataLookUpResult + "').typeahead({");
                        sbJS.AppendLine("                highlight: true,");
                        sbJS.AppendLine("                minLength: " + (item.FilterType == TableColumnFilterType.DropDown ? "0" : "1") + ",");
                        sbJS.AppendLine("            },");
                        sbJS.AppendLine("            {");
                        sbJS.AppendLine("                name: 'th" + item.DataLookUpResult + "',");
                        sbJS.AppendLine("                display: 'Name',");
                        sbJS.AppendLine("                highlight: true,");
                        sbJS.AppendLine("                limit: 30,");
                        sbJS.AppendLine("                source: source" + item.DataLookUpResult + ",");
                        sbJS.AppendLine("            });");
                        sbJS.AppendLine();
                        sbJS.AppendLine("            $(\"#" + item.DataLookUpResult + "\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#" + item.DataNameSql + "\").val(datum.ID); });");
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
                if (item.DataType != TableColumnType.Files && item.DataType != TableColumnType.Text)
                    sb.AppendLine("               <th>" + (string.IsNullOrEmpty(item.ViewCaptionShort) ? item.ViewCaption : item.ViewCaptionShort) + "</th>");
            }
            sb.AppendLine("            </tr>");
            sb.AppendLine("        </thead>");
            sb.AppendLine("    </table>");

            return sb.ToString();
        }

        // Плюсик
        public virtual string GenerateJSTableInfoButton()
        {
            return string.Empty;
        }

        // Инфо под плюсиком
        public virtual string GenerateJSTableInfoButtonContent(string id)
        {
            return string.Empty;
        }

        // Grid
        public string GenerateJSDataTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("            var table = $('#table" + TableSql + "').DataTable({");
            sb.AppendLine("                dom: '<\"row top-toolbar\"<\"col-sm-4\"B><\"col-sm-4\"><\"col-sm-4\"pi>><\"row top-filterbar\">Zrt',");
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
            sb.AppendLine();
            if (ShowRowInfoButtom)
                sb.AppendLine(GenerateJSTableInfoButton());

            return sb.ToString();
        }

        // Editor
        public string GenerateJSEditorInit()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("            editor = new $.fn.dataTable.Editor({");
            sb.AppendLine("                ajax: \"/Handler/GridSaveDataHandler.ashx?curBase=" + SqlBase + "&curTable=" + TableSql + "&curPage=" + PageName + "\",");
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
                    switch (column.EditType)
                    {
                        case TableColumnEditType.Autocomplete:
                        case TableColumnEditType.DropDown:
                            sb.AppendLine("                             name: \"" + column.DataLookUpResult + "\",");// Для join-ов показываем результат join-а
                            break;

                        default:
                            sb.AppendLine("                             name: \"" + column.DataNameSql + "\",");
                            break;
                    }

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
        }

        // Кнопоки грида
        public string GenerateJSTableButtons()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("                    {");
            sb.AppendLine("                        text: '<span class=\"glyphicon glyphicon-plus\" title=\"Создать новую запись\"></span>',");
            sb.AppendLine("                        className: 'btn-sm',");
            sb.AppendLine("                        action: function (e, dt, node, config) {");
            sb.AppendLine("                            $('#EditDialog').modal();");
            sb.AppendLine("                            $('#EditDialogContent').html('Загрузка..');");
            sb.AppendLine("                            $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=" + SqlBase + "&curTable=" + TableSql + "&curPage=" + PageName + "&action=create&curId=0&_=' + (new Date()).getTime());");
            sb.AppendLine("                        },");
            sb.AppendLine("                        key: \"n\",");
            sb.AppendLine("                        className: \"btn-sm\",");
            sb.AppendLine("                    },");
            // sb.AppendLine("                    { extend: 'create', editor: editor, className: 'btn-sm', key: \"l\", text: '<span class=\"glyphicon glyphicon-plus\" title=\"Создать новую запись\"></span>' },");
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
            sb.AppendLine("                                 extend: 'collection',");
            sb.AppendLine("                                 text: 'Выбрать представление',");
            sb.AppendLine("                                 buttons: [");
            sb.AppendLine("                                     {");
            sb.AppendLine("                                         text: 'Общие',");
            sb.AppendLine("                                         action: function (e, dt, node, config) {");
            sb.AppendLine("                                             dt.state.clear();");
            sb.AppendLine("                                             window.location.reload();");
            sb.AppendLine("                                         },");
            sb.AppendLine("                                     },");
            sb.AppendLine("                                     {");
            sb.AppendLine("                                         text: 'Бухгалтерские документы',");
            sb.AppendLine("                                         action: function (e, dt, node, config) {");
            sb.AppendLine("                                             dt.state.clear();");
            sb.AppendLine("                                             window.location.reload();");
            sb.AppendLine("                                         },");
            sb.AppendLine("                                     },");
            sb.AppendLine("                                     {");
            sb.AppendLine("                                         text: 'Договоры',");
            sb.AppendLine("                                         action: function (e, dt, node, config) {");
            sb.AppendLine("                                             dt.state.clear();");
            sb.AppendLine("                                             window.location.reload();");
            sb.AppendLine("                                         },");
            sb.AppendLine("                                     },");
            sb.AppendLine("                                 ],");
            sb.AppendLine("                            },");
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
            sb.AppendLine("                            },");
            
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

                switch (item.DataType)
                {
                    case TableColumnType.Files:
                    case TableColumnType.Text:
                        break;

                    case TableColumnType.LookUp:
                        sb.AppendLine("                    { \"data\": \"" + item.DataLookUpResult + "\", className:\"dt-body-" + item.ViewAlign.ToString().ToLower() + "\", \"width\": \"" + item.ViewWidth + "px\" },");// Для join-ов показываем результат join-а
                        break;

                    default:
                        sb.AppendLine("                    { \"data\": \"" + item.DataNameSql + "\", className:\"dt-body-" + item.ViewAlign.ToString().ToLower() + "\", \"width\": \"" + item.ViewWidth + "px\" },");
                        break;
                }
            return sb.ToString();
        }

        // Список количества записей на странице
        public string GenerateJSTableLengthMenu()
        {
            return "[30, 100, 200, 500], ['30 строк', '100 строк', '200 строк', '500 строк']";
        }

        #endregion Генерация HTML | JS

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
                            key = item.DataNameSql;
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

        // Возвращает текст текущей версии
        // id - id Карточки
        public virtual string GetText(string id)
        {
            // Запрос
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("SELECT [Text] ");
            sbQuery.AppendLine("FROM [dbo].[" + SqlBase + TableSql + "Text]");
            sbQuery.AppendLine("WHERE IdArchive = " + id);

            // Выполняем запрос
            var res = ComFunc.ExecuteScalar(sbQuery.ToString());
            if (res is DBNull || res == null)
                return string.Empty;
            else
                return res.ToString();
        }

        // Возвращает список файлов для текущей версии или ID
        // idVer - id версии
        public virtual DataTable GetFileList(string idVer, string Id = "0")
        {
            // Запрос
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("SELECT T.IdFile, F.fileName, F.IsPrivate ");
            if (Id == "0")
            {
                sbQuery.AppendLine("FROM [dbo].[" + SqlBase + TableSql + "FileList] T");
                sbQuery.AppendLine("JOIN [dbo].[" + SqlBase + TableSql + "Files] F ON F.Id = T.IdFile");
                sbQuery.AppendLine("WHERE T.IdVer = " + idVer);
            }
            else
            {
                sbQuery.AppendLine("FROM [dbo].[" + SqlBase + TableSql + "] A");
                sbQuery.AppendLine("JOIN [dbo].[" + SqlBase + TableSql + "FileList] T ON T.IdVer=A.IdVer");
                sbQuery.AppendLine("JOIN [dbo].[" + SqlBase + TableSql + "Files] F ON F.Id = T.IdFile");
                sbQuery.AppendLine("WHERE A.Active=1 AND A.Del=0 AND A.Id = " + Id);
            }

            DataTable dt = ComFunc.GetData(sbQuery.ToString());
            return dt;
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

                        case TableColumnType.LookUp:
                            row.Add(column.DataLookUpResult, dr[column.DataLookUpResult].ToString());
                            break;

                        case TableColumnType.String:
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
                    case TableAction.Copy:
                        // Для каждой переданной строки с данными, создаем строку запроса и параметры к ней, выполняем запрос
                        foreach (KeyValuePair<string, List<RequestData>> pair in RequestRows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            // Обновляем запись в главной таблице
                            query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "]");
                            query.AppendLine("    ([DateUpd]"); // Дата внесения
                            query.AppendLine("    ,[IdUser]"); // Пользователь внесший изменения

                            foreach (RequestData rd in pair.Value)
                            {
                                TableColumn tableColumn = ColumnList.Find(x => x.DataNameSql == rd.FieldName);
                                if (tableColumn != null && tableColumn.EditType != TableColumnEditType.Text) // Текст сохраняем отдельно
                                {
                                    query.AppendLine("    ,[" + rd.FieldName + "]");
                                }
                            }
                            query.AppendLine("    )");

                            query.AppendLine("VALUES ");
                            query.AppendLine("    (GetDate()");
                            query.AppendLine("    ,@IdUser");

                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = HttpContext.Current.Session["UserId"].ToString() });
                            foreach (RequestData rd in pair.Value)
                            {
                                TableColumn tableColumn = ColumnList.Find(x => x.DataNameSql == rd.FieldName);

                                if (tableColumn != null && tableColumn.EditType != TableColumnEditType.Text)
                                {
                                    query.AppendLine("    ,@" + rd.FieldName);
                                    SqlDbType sqlDbType = SqlDbType.NVarChar;
                                    object value = rd.FieldValue;

                                    switch (tableColumn.DataType)
                                    {
                                        case TableColumnType.Integer:
                                            sqlDbType = SqlDbType.Int;
                                            break;

                                        case TableColumnType.Money:
                                            sqlDbType = SqlDbType.Decimal;
                                            break;

                                        case TableColumnType.DateTime:
                                            sqlDbType = SqlDbType.DateTime;
                                            if (string.IsNullOrEmpty(rd.FieldValue))
                                                value = DBNull.Value;
                                            break;

                                        case TableColumnType.Date:
                                            sqlDbType = SqlDbType.Date;
                                            if (string.IsNullOrEmpty(rd.FieldValue))
                                                value = DBNull.Value;
                                            break;

                                        case TableColumnType.LookUp:
                                            sqlDbType = SqlDbType.Int;
                                            break;
                                    }
                                    param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = sqlDbType, Value = value });
                                }
                            }
                            query.AppendLine("    );");
                            query.AppendLine();
                            query.AppendLine("DECLARE @si AS int;");
                            query.AppendLine("SET @si = SCOPE_IDENTITY();");
                            query.AppendLine("UPDATE[dbo].[" + SqlBase + TableSql + "] SET[Id] = [IdVer] WHERE IdVer = @si;");
                            query.AppendLine();

                            // Сохраняем файлы
                            if (RequestFiles != null)
                            {
                                query.AppendLine("DECLARE @fileId AS int;");
                                for (int i = 0; i < RequestFiles.Count; i++)
                                {
                                    HttpPostedFile file = RequestFiles[i];

                                    if (file.ContentLength > 0)
                                    {
                                        byte[] fileData = null;
                                        using (var binaryReader = new BinaryReader(file.InputStream))
                                        {
                                            fileData = binaryReader.ReadBytes(file.ContentLength);
                                        }
                                        query.AppendLine();
                                        query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "Files]([fileDATA],[fileName])VALUES(@fileData" + i + ",@fileName" + i + ");");// Записали файл
                                        query.AppendLine("SET @fileId = SCOPE_IDENTITY();");// Узнали его id
                                        query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "FileList]([IdVer],[IdFile])VALUES(@si,@fileId);");

                                        param.Add(new SqlParameter { ParameterName = "@fileName" + i, SqlDbType = SqlDbType.NVarChar, Value = Path.GetFileName(file.FileName).Trim() });
                                        param.Add(new SqlParameter { ParameterName = "@fileData" + i, SqlDbType = SqlDbType.VarBinary, Value = fileData });
                                    }
                                }
                            }

                            // Сохраняем текст
                            foreach (RequestData rd in pair.Value)
                            {
                                TableColumn tableColumn = ColumnList.Find(x => x.DataNameSql == rd.FieldName);
                                if (tableColumn!=null && tableColumn.EditType == TableColumnEditType.Text)
                                {
                                    query.AppendLine();
                                    query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "Text]([IdArchive],[Text])VALUES(@si,@Text);"); // Записали файл
                                    param.Add(new SqlParameter { ParameterName = "@Text", SqlDbType = SqlDbType.Text, Value = rd.FieldValue });
                                }
                            }

                            query.AppendLine("SELECT @si;");

                            sqlCommand = new SqlCommand(query.ToString(), sqlConnection, sqlTransaction);
                            sqlCommand.Parameters.AddRange(param.ToArray());
                            result = sqlCommand.ExecuteScalar().ToString(); // Получаем Id новой записи
                        }
                        break;

                    case TableAction.Edit:
                        // Для каждой переданной строки с данными, создаем строку запроса и параметры к ней, выполняем запрос
                        foreach (KeyValuePair<string, List<RequestData>> pair in RequestRows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            query.AppendLine("DECLARE @nextVer AS int;");
                            query.AppendLine("DECLARE @prevVer AS int;");

                            // Номер текущей версии
                            query.AppendLine("SELECT @prevVer = IdVer FROM [dbo].[" + SqlBase + TableSql + "] WHERE Id = @Id AND [Active]=1;");

                            // Снимаем активность предыдущих записей
                            query.AppendLine("UPDATE[dbo].[" + SqlBase + TableSql + "] SET [Active] = 0 WHERE Id = @Id AND [Active]=1;");
                            query.AppendLine();

                            // Добавляем новую версию
                            query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "]");
                            query.AppendLine("    ([Id]");
                            query.AppendLine("    ,[DateUpd]"); // Дата внесения
                            query.AppendLine("    ,[IdUser]"); // Пользователь внесший изменения

                            foreach (RequestData rd in pair.Value)
                            {
                                TableColumn tableColumn = ColumnList.Find(x => x.DataNameSql == rd.FieldName);
                                if (tableColumn != null && tableColumn.EditType != TableColumnEditType.Text)
                                {
                                    query.AppendLine("    ,[" + rd.FieldName + "]");
                                }
                            }
                            query.AppendLine("    )");

                            query.AppendLine("VALUES ");
                            query.AppendLine("    (@Id");
                            query.AppendLine("    ,GetDate()");
                            query.AppendLine("    ,@IdUser");

                            param.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = pair.Key });
                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = HttpContext.Current.Session["UserId"].ToString() });
                            foreach (RequestData rd in pair.Value)
                            {
                                TableColumn tableColumn = ColumnList.Find(x => x.DataNameSql == rd.FieldName);
                                if (tableColumn != null && tableColumn.EditType != TableColumnEditType.Text)
                                {
                                    query.AppendLine("    ,@" + rd.FieldName);
                                    SqlDbType sqlDbType = SqlDbType.NVarChar;
                                    object value = rd.FieldValue;

                                    if (tableColumn != null)
                                    {
                                        switch (tableColumn.DataType)
                                        {
                                            case TableColumnType.Integer:
                                                sqlDbType = SqlDbType.Int;
                                                break;

                                            case TableColumnType.Money:
                                                sqlDbType = SqlDbType.Decimal;
                                                break;

                                            case TableColumnType.DateTime:
                                                sqlDbType = SqlDbType.DateTime;
                                                if (string.IsNullOrEmpty(rd.FieldValue))
                                                    value = DBNull.Value;
                                                break;

                                            case TableColumnType.Date:
                                                sqlDbType = SqlDbType.Date;
                                                if (string.IsNullOrEmpty(rd.FieldValue))
                                                    value = DBNull.Value;
                                                break;

                                            case TableColumnType.LookUp:
                                                sqlDbType = SqlDbType.Int;
                                                break;
                                        }
                                    }
                                    param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = sqlDbType, Value = value });
                                }
                            }
                            query.AppendLine("    );");
                            query.AppendLine();
                            query.AppendLine("SET @nextVer = SCOPE_IDENTITY();"); // Узнали номер следующей версии

                            // Сохраняем файлы
                            if (RequestFiles != null)
                            {
                                query.AppendLine("DECLARE @fileId AS int;");
                                for (int i = 0; i < RequestFiles.Count; i++)
                                {
                                    HttpPostedFile file = RequestFiles[i];
                                    if (file.ContentLength > 0)
                                    {
                                        byte[] fileData = null;
                                        using (var binaryReader = new BinaryReader(file.InputStream))
                                        {
                                            fileData = binaryReader.ReadBytes(file.ContentLength);
                                        }
                                        query.AppendLine();
                                        query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "Files]([fileDATA],[fileName])VALUES(@fileData" + i + ",@fileName" + i + ");");// Записали файл
                                        query.AppendLine("SET @fileId = SCOPE_IDENTITY();");// Узнали его id
                                        query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "FileList]([IdVer],[IdFile])VALUES(@nextVer,@fileId);");

                                        param.Add(new SqlParameter { ParameterName = "@fileName" + i, SqlDbType = SqlDbType.NVarChar, Value = Path.GetFileName(file.FileName) });
                                        param.Add(new SqlParameter { ParameterName = "@fileData" + i, SqlDbType = SqlDbType.VarBinary, Value = fileData });
                                    }
                                }
                            }

                            string toPrivate, toDelete = string.Empty;
                            toPrivate = RequestRows[pair.Key].Find(x => x.FieldName == "FilesToPrivate").FieldValue;
                            toDelete = RequestRows[pair.Key].Find(x => x.FieldName == "FilesToDelete").FieldValue;

                            if (!string.IsNullOrEmpty(toPrivate))
                            {
                                query.AppendLine("UPDATE [dbo].[" + SqlBase + TableSql + "Files] SET [IsPrivate] = 1 WHERE [ID] in (" + toPrivate + ");");
                            }
                            query.AppendLine();

                            // Копируем файлы из предыдущей версии без удаленных
                            query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "FileList]([IdVer],[IdFile]) ");
                            query.AppendLine("      SELECT @nextVer, IdFile FROM [dbo].[" + SqlBase + TableSql + "FileList]");
                            query.AppendLine("      WHERE IdVer=@prevVer" + (string.IsNullOrEmpty(toDelete) ? "" : " AND [IdFile] NOT IN (" + toDelete + ")") + ";");

                            // Сохраняем текст
                            foreach (RequestData rd in pair.Value)
                            {
                                TableColumn tableColumn = ColumnList.Find(x => x.DataNameSql == rd.FieldName);
                                if (tableColumn != null && tableColumn.EditType == TableColumnEditType.Text && rd.FieldValue.Length > 0)
                                {
                                    query.AppendLine();
                                    query.AppendLine("UPDATE [dbo].[" + SqlBase + TableSql + "Text] SET [Text]=@Text WHERE [IdArchive]=" + pair.Key+" and CAST([Text] as nvarchar(max))<>@Text");
                                    param.Add(new SqlParameter { ParameterName = "@Text", SqlDbType = SqlDbType.NVarChar, Value = rd.FieldValue });
                                }
                            }

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

                            // Снимаем активность предыдущих записей
                            query.AppendLine("UPDATE[dbo].[" + SqlBase + TableSql + "] SET [Active] = 0, [Del] = 1 WHERE Id = @Id;");
                            query.AppendLine();

                            // Добавляем новую версию
                            query.AppendLine("INSERT INTO [dbo].[" + SqlBase + TableSql + "]");
                            query.AppendLine("    ([Id]");
                            query.AppendLine("    ,[DateUpd]"); // Дата внесения
                            query.AppendLine("    ,[IdUser]"); // Пользователь внесший изменения
                            query.AppendLine("    ,[Del]"); // Пользователь внесший изменения
                            query.AppendLine("    )");

                            query.AppendLine("VALUES ");
                            query.AppendLine("    (@Id");
                            query.AppendLine("    ,GetDate()");
                            query.AppendLine("    ,@IdUser");
                            query.AppendLine("    ,@Del");

                            param.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = pair.Key });
                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = HttpContext.Current.Session["UserId"].ToString() });
                            param.Add(new SqlParameter { ParameterName = "@Del", SqlDbType = SqlDbType.Bit, Value = true });
                            query.AppendLine("    );");

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

                ComFunc.LogSqlError(ex, sqlCommand.CommandText, param.ToArray());
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
                    if (tableColumn != null)
                    {
                        // Обязательность заполнения поля
                        if (resume && tableColumn.EditRequired)
                        {
                            // Для текстовых/числовых не пропускаем пустые строки, для справочников не пропускаем еще и нули
                            if (string.IsNullOrEmpty(rd.FieldValue) || ((tableColumn.EditType == TableColumnEditType.Autocomplete || tableColumn.EditType == TableColumnEditType.DropDown) && rd.FieldValue == "0"))
                            {
                                fieldErrors.Add(new FieldErrors { name = tableColumn.DataNameSql, status = "Поле обязательно для заполнения" });
                                resume = false;
                            }
                        }

                        // Проверяем тип введенных данных
                        if (resume)
                        {
                            switch (tableColumn.EditType)
                            {
                                case TableColumnEditType.Date:
                                    if (!string.IsNullOrEmpty(rd.FieldValue))
                                    {
                                        DateTime date;
                                        if (!DateTime.TryParse(rd.FieldValue, out date))
                                        {
                                            fieldErrors.Add(new FieldErrors { name = tableColumn.DataNameSql, status = "Неверный формат даты" });
                                            resume = false;
                                        }
                                        else if (date > new DateTime(2020, 1, 1) || date < new DateTime(2000, 1, 1))
                                        {
                                            fieldErrors.Add(new FieldErrors { name = tableColumn.DataNameSql, status = "Выбрана недопустимая дата" });
                                            resume = false;
                                        }
                                        else rd.FieldValue = date.ToString("yyyy-MM-dd");
                                    }
                                    break;

                                case TableColumnEditType.Autocomplete:
                                case TableColumnEditType.DropDown:
                                case TableColumnEditType.Integer:
                                    if (string.IsNullOrEmpty(rd.FieldValue))
                                        rd.FieldValue = "0";
                                    break;

                                case TableColumnEditType.Money:
                                    decimal money;
                                    rd.FieldValue = rd.FieldValue.Replace('.', ',');
                                    if (!decimal.TryParse(rd.FieldValue, out money))
                                    {
                                        fieldErrors.Add(new FieldErrors { name = tableColumn.DataNameSql, status = "Неверный формат суммы" });
                                        resume = false;
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }

                        // Ограничения
                        if (resume)
                        {
                            switch (tableColumn.EditType)
                            {
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