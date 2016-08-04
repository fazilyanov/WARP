using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Script.Serialization;

namespace WARP
{
    public class AppPage
    {
        // Текст на вкладке браузера
        public string BrowserTabTitle { get; set; } = string.Empty;

        // Ширина карточки
        public int EditDialogWidth { get; set; } = 0;

        // Ширина карточки
        public int EditDialogTabHeight { get; set; } = 300;

        // Грид шапки
        public TableData Master { get; set; } = null;

        // Список подчиненных шапке таблиц
        public List<TableData> Detail { get; set; } = null;

        // Скрипт бинда изменения размеров рабочей области
        public virtual string GenerateJSWindowsResize()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("        $(window).bind('resize', function () {");
            sb.AppendLine("            $('.dataTables_scrollBody').css('height', ($(window).height() - 125) + 'px');");
            sb.AppendLine("        });");
            return sb.ToString();
        }

        public virtual string MouseDoubleClick()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("            $('#table" + Master.TableSql + "').on('dblclick', 'tr', function() {");
            sb.AppendLine("                var table = $('#table" + Master.TableSql + "').DataTable();");
            sb.AppendLine("                var id = table.row(this).id();");
            sb.AppendLine("                $('#EditDialog').modal();");
            // sb.AppendLine("                $('#EditDialogContent').html('Загрузка..');");
            sb.AppendLine("                $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=" + Master.SqlBase + "&curTable=" + Master.TableSql + "&curPage=" + Master.PageName + "&action=edit&curId=' + id +'&_=' + (new Date()).getTime());");
            sb.AppendLine("            });");
            return sb.ToString();
        }

        public virtual string ContextMenu()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("            $.contextMenu({");
            sb.AppendLine("                selector: '#table" + Master.TableSql + " .selected td',");
            sb.AppendLine("                items: {");
            sb.AppendLine("                    foo: {");
            sb.AppendLine("                        name: 'Foo',");
            sb.AppendLine("                        callback: function(key, opt) {");
            sb.AppendLine("                            alert('Foo!');");
            sb.AppendLine("                        }");
            sb.AppendLine("                    },");
            sb.AppendLine("                    bar: {");
            sb.AppendLine("                        name: 'Bar',");
            sb.AppendLine("                        callback: function(key, opt) {");
            sb.AppendLine("                            alert('Bar!')");
            sb.AppendLine("                        }");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            });");

            return sb.ToString();
        }

        public virtual string GenerateEditDialog(string curId)
        {
            DataRow data = null;
            switch (Master.Action)
            {
                case TableAction.Edit:
                case TableAction.Copy:
                    DataTable dt = Master.GetData(curId);
                    if (dt.Rows.Count == 0)
                    {
                        return "Not found id = " + curId;
                    }
                    else
                    {
                        data = dt.Rows[0];
                    }
                    break;

                default:
                    break;
            }

            StringBuilder sb = new StringBuilder();

            #region Шапка Диалога

            sb.AppendLine("<div class=\"card-modal-header\">");
            sb.AppendLine("     <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>");
            sb.AppendLine("     <div id=\"HeaderMsg\" class=\"label label-success card-modal-header-msg\"></div>");
            sb.AppendLine("     <div id=\"HeaderError\" class=\"label label-danger card-modal-header-msg\"></div>");
            switch (Master.Action)
            {
                case TableAction.Create:
                case TableAction.Copy:
                    sb.AppendLine("     <h4 class=\"modal-title\">Новая запись</h4>");
                    break;

                case TableAction.Edit:
                case TableAction.Remove:
                    sb.AppendLine("     <h4 class=\"modal-title\">Запись № " + curId.ToString() + "</h4>");
                    sb.AppendLine("     <h6 class=\"modal-title\">Дата редактирования: " + data["DateUpd"].ToString() + " " + data["User"].ToString() + "</h6>");
                    break;

                default:
                    break;
            }
            sb.AppendLine("</div>");

            #endregion Шапка Диалога

            #region Шапка документа

            sb.AppendLine("<form method=\"POST\" id=\"EditForm\" name=\"EditForm\" action=\"javascript: void(null);\" enctype=\"multipart/form-data\">");
            sb.AppendLine("<div id=\"EditDialogBody\" class=\"modal-body\">");
            sb.AppendLine("     <div class=\"row\">");

            string value = string.Empty;
            string valueText = string.Empty;
            StringBuilder js = new StringBuilder();
            foreach (TableColumn tableColumn in Master.ColumnList)
            {
                // Текущие или стандарные значения для инпута
                if (tableColumn.EditType != TableColumnEditType.None && tableColumn.EditType != TableColumnEditType.Files && tableColumn.EditType != TableColumnEditType.Text)
                    switch (Master.Action)
                    {
                        case TableAction.Create:
                            value = tableColumn.EditDefaultValue;
                            valueText = tableColumn.EditDefaultText;///!!!
                            break;

                        case TableAction.Edit:
                        case TableAction.Copy:
                            value = data[tableColumn.DataNameSql].ToString();
                            if (tableColumn.EditType == TableColumnEditType.Autocomplete || tableColumn.EditType == TableColumnEditType.DropDown)
                                valueText = (data[tableColumn.DataLookUpResult] ?? string.Empty).ToString();
                            else
                                valueText = string.Empty;
                            break;
                    }

                // Выбираем тип, генерим инпут
                switch (tableColumn.EditType)
                {
                    case TableColumnEditType.Date:
                        sb.AppendLine("             <div class=\"card-input-group\">");
                        sb.AppendLine("                 <label class=\"card-label\" >" + tableColumn.ViewCaption + "</label>");
                        sb.AppendLine("                     <input id=\"" + tableColumn.DataNameSql + "\" name=\"" + tableColumn.DataNameSql + "\" class=\"card-form-control\" value=\"" + value + "\" >");
                        sb.AppendLine("                 <div id=\"" + tableColumn.DataNameSql + "Error\" class=\"card-input-error\">&nbsp;</div>");
                        sb.AppendLine("             </div>");
                        js.AppendLine("$('#" + tableColumn.DataNameSql + "').mask('99.99.9999',{ placeholder: 'дд.мм.гггг'}); ");
                        js.AppendLine("$('#" + tableColumn.DataNameSql + "').datetimepicker({locale: 'ru', useCurrent:false, format: 'DD.MM.YYYY',}); ");
                        break;

                    case TableColumnEditType.String:
                        sb.AppendLine("             <div class=\"card-input-group\">");
                        sb.AppendLine("                 <label class=\"card-label\" >" + tableColumn.ViewCaption + "</label>");
                        sb.AppendLine("                 <input id=\"" + tableColumn.DataNameSql + "\" name=\"" + tableColumn.DataNameSql + "\" class=\"card-form-control\" value=\"" + value + "\" >");
                        sb.AppendLine("                 <div id=\"" + tableColumn.DataNameSql + "Error\" class=\"card-input-error\">&nbsp;</div>");
                        sb.AppendLine("             </div>");
                        break;

                    case TableColumnEditType.DropDown:
                    case TableColumnEditType.Autocomplete:
                        sb.AppendLine("             <div id=\"scrollable-dropdown-menu\">");
                        sb.AppendLine("                 <div class=\"card-input-group\">");
                        sb.AppendLine("                     <label class=\"card-label\" >" + tableColumn.ViewCaption + "</label>");
                        sb.AppendLine("                     <input type=\"text\"  id=\"" + tableColumn.DataLookUpResult + "\" onchange=\"if ($('#" + tableColumn.DataLookUpResult + "').val().trim() == '')$('#" + tableColumn.DataNameSql + "').val(0);\" ");
                        sb.AppendLine("                         class=\"card-form-control\"  value=\"" + valueText + "\" placeholder=\"Начните вводить для поиска по справочнику..\">");
                        sb.AppendLine("                     <input type=\"hidden\" id=\"" + tableColumn.DataNameSql + "\" name=\"" + tableColumn.DataNameSql + "\" value=\"" + value + "\">");
                        sb.AppendLine("                     <div id=\"" + tableColumn.DataNameSql + "Error\" class=\"card-input-error\">&nbsp;</div>");
                        sb.AppendLine("                 </div>");
                        sb.AppendLine("             </div>");

                        js.AppendLine();
                        js.AppendLine("            // Для столбца: " + tableColumn.ViewCaption);
                        js.AppendLine("            var source" + tableColumn.DataLookUpResult + " = new Bloodhound({");
                        js.AppendLine("                datumTokenizer: Bloodhound.tokenizers.whitespace,");
                        js.AppendLine("                queryTokenizer: Bloodhound.tokenizers.whitespace,");
                        js.AppendLine("                remote: {");
                        js.AppendLine("                    url: '/Handler/TypeaheadHandler.ashx?t=" + tableColumn.DataLookUpTable + "&q=%QUERY',");
                        js.AppendLine("                    wildcard: '%QUERY'");
                        js.AppendLine("                },");
                        js.AppendLine("                limit: 30,");
                        js.AppendLine("            });");
                        js.AppendLine();
                        js.AppendLine("            $('#scrollable-dropdown-menu #" + tableColumn.DataLookUpResult + "').typeahead({");
                        js.AppendLine("                highlight: true,");
                        js.AppendLine("                minLength: " + (tableColumn.FilterType == TableColumnFilterType.DropDown ? "0" : "1") + ",");
                        js.AppendLine("            },");
                        js.AppendLine("            {");
                        js.AppendLine("                name: 'th" + tableColumn.DataLookUpResult + "',");
                        js.AppendLine("                display: 'Name',");
                        js.AppendLine("                highlight: true,");
                        js.AppendLine("                limit: 30,");
                        js.AppendLine("                source: source" + tableColumn.DataLookUpResult + ",");
                        js.AppendLine("            });");
                        js.AppendLine();
                        js.AppendLine("            $(\"#" + tableColumn.DataLookUpResult + "\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#" + tableColumn.DataNameSql + "\").val(datum.ID); });");
                        break;

                    case TableColumnEditType.Integer:
                        sb.AppendLine("             <div class=\"card-input-group\">");
                        sb.AppendLine("                 <label class=\"card-label\" >" + tableColumn.ViewCaption + "</label>");
                        sb.AppendLine("                     <input id=\"" + tableColumn.DataNameSql + "\" name=\"" + tableColumn.DataNameSql + "\" class=\"card-form-control\" value=\"" + value + "\" >");
                        sb.AppendLine("                 <div id=\"" + tableColumn.DataNameSql + "Error\" class=\"card-input-error\">&nbsp;</div>");
                        sb.AppendLine("             </div>");
                        break;

                    case TableColumnEditType.Money:
                        sb.AppendLine("             <div class=\"card-input-group\">");
                        sb.AppendLine("                 <label class=\"card-label\" >" + tableColumn.ViewCaption + "</label>");
                        sb.AppendLine("                 <input id=\"" + tableColumn.DataNameSql + "\" name=\"" + tableColumn.DataNameSql + "\" class=\"card-form-control\" value=\"" + value + "\" >");
                        sb.AppendLine("                 <div id=\"" + tableColumn.DataNameSql + "Error\" class=\"card-input-error\">&nbsp;</div>");
                        sb.AppendLine("             </div>");
                        js.AppendLine();
                        js.AppendLine("             $('#" + tableColumn.DataNameSql + "').val(accounting.formatNumber($('#" + tableColumn.DataNameSql + "').val().trim().replace(',', '.'), 2, ' '));");
                        js.AppendLine("             $('#" + tableColumn.DataNameSql + "').bind('blur', function(event) {");
                        js.AppendLine("                 this.value = accounting.formatNumber(this.value.trim().replace(',', '.'), 2, ' ');");
                        js.AppendLine("             });");
                        js.AppendLine("             $('#" + tableColumn.DataNameSql + "').bind('focus', function(event) {");
                        js.AppendLine("                 this.value = accounting.unformat(this.value.trim());");
                        js.AppendLine("             });");
                        break;

                    default:
                        break;
                }
            }
            sb.AppendLine("     </div>");// row
            sb.AppendLine();

            #endregion Шапка документа

            #region Табличная часть

            sb.AppendLine("     <div>");
            StringBuilder li = new StringBuilder(); // Вкладки
            StringBuilder tp = new StringBuilder(); // Содержимое
            foreach (TableColumn tableColumn in Master.ColumnList)
            {
                #region Вкладка Файлы

                if (tableColumn.DataType == TableColumnType.Files)
                {
                    li.AppendLine("			  <li><a data-target=\"#" + tableColumn.DataNameSql + "Tab\" data-toggle=\"tab\">" + tableColumn.ViewCaption + "</a></li>");
                    tp.AppendLine("           <div role=\"tabpanel\" class=\"tab-pane fade\" id=\"" + tableColumn.DataNameSql + "Tab\" style=\"height: " + EditDialogTabHeight + "px;\">");
                    tp.AppendLine("                 <label class=\"btn btn-primary btn-file\">");
                    tp.AppendLine("                     Добавить&nbsp;<span id=\"badge\" class=\"badge\"></span><input id=\"" + tableColumn.DataNameSql + "\" name=\"" + tableColumn.DataNameSql + "\" type=\"file\" multiple/ onchange=\"$('#badge').html('Файлов:'+$('#Files').get(0).files.length);\">");
                    tp.AppendLine("                 </label>");
                    tp.AppendLine("                 <div style=\"width: 470px;display: table;margin-top:10px;\">");
                    tp.AppendLine("                 <input type=\"hidden\" id=\"" + tableColumn.DataNameSql + "ToPrivate\" name=\"" + tableColumn.DataNameSql + "ToPrivate\" value=\"0\">");
                    tp.AppendLine("                 <input type=\"hidden\" id=\"" + tableColumn.DataNameSql + "ToDelete\" name=\"" + tableColumn.DataNameSql + "ToDelete\" value=\"0\">");

                    if (Master.Action != TableAction.Create && Master.Action != TableAction.Copy) // Если карточка не новая | скопированная
                    {
                        DataTable dtFiles = Master.GetFileList(data["IdVer"].ToString()); // Получаем список файлов
                        if (dtFiles.Rows.Count > 0)
                        {
                            string fn, fid, hash = string.Empty;
                            bool isPrivate = false;
                            foreach (DataRow row in dtFiles.Rows)
                            {
                                fid = row["IdFile"].ToString();
                                fn = row["fileName"].ToString().Trim();
                                hash = ComFunc.GetMd5Hash(ComFunc.GetMd5Hash(fid) + fid);
                                isPrivate = (bool)row["IsPrivate"];

                                tp.AppendLine("                  <div class=\"file-button\"  id=\"FileButton" + fid + "\">");
                                tp.AppendLine("                     <div class=\"btn-group\">");
                                tp.AppendLine("                         <button type=\"button\"  class=\"btn btn-default " + (isPrivate ? "opacity" : "") + "\" style=\"width:200px;\" title=\"" + fn + "\"");
                                tp.AppendLine("                              onclick =\"window.open('/Handler/GetFileHandler.ashx?curBase=" + Master.SqlBase + "&curTable=" + Master.TableSql + "&IdFile=" + row["IdFile"].ToString() + "&key=" + hash + "');\" >" + (fn.Length > 24 ? fn.Substring(0, 22) + ".." : fn) + "</button>");//
                                tp.AppendLine("                         <button type=\"button\" class=\"btn btn-default dropdown-toggle " + (isPrivate ? "opacity" : "") + "\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">");
                                tp.AppendLine("                             <span class=\"caret\"></span>");
                                tp.AppendLine("                             <span class=\"sr-only\">Toggle Dropdown</span>");
                                tp.AppendLine("                         </button>");
                                tp.AppendLine("                         <ul class=\"dropdown-menu\">");
                                tp.AppendLine("                             <li><a href=\"#\" onclick=\"var tp = $('#" + tableColumn.DataNameSql + "ToPrivate'); tp.val(tp.val()+'," + fid + "');$('#FileButton" + fid + " .btn').animate({opacity: 0.5}); AllowSave();\">Скрыть</a></li>");
                                tp.AppendLine("                              <li><a href=\"#\" onclick=\"var tp = $('#" + tableColumn.DataNameSql + "ToDelete'); tp.val(tp.val()+'," + fid + "');$('#FileButton" + fid + "').fadeOut();AllowSave();\">Удалить</a></li>");
                                tp.AppendLine("                         </ul>");
                                tp.AppendLine("                     </div>");
                                tp.AppendLine("                  </div>");
                            }
                        }
                    }
                    tp.AppendLine("                 </div>");
                    tp.AppendLine("           </div>");
                }

                #endregion Вкладка Файлы

                #region Вкладка Текст

                if (tableColumn.DataType == TableColumnType.Text)
                {
                    li.AppendLine("			  <li><a data-target=\"#" + tableColumn.DataNameSql + "Tab\" data-toggle=\"tab\">" + tableColumn.ViewCaption + "</a></li>");
                    tp.AppendLine("           <div role=\"tabpanel\" class=\"tab-pane fade\" id=\"" + tableColumn.DataNameSql + "Tab\" style=\"height: " + EditDialogTabHeight + "px;\">");
                    string text = string.Empty;
                    if (Master.Action != TableAction.Create && Master.Action != TableAction.Copy) // Если карточка не новая
                        text = Master.GetText(data["Id"].ToString()); // Получаем текст для версии
                    tp.AppendLine("                 <textarea id=\"" + tableColumn.DataNameSql + "\" name=\"" + tableColumn.DataNameSql + "\" class=\"card-form-control card-textarea\">" + text + "</textarea>");
                    tp.AppendLine("           </div>");
                }

                #endregion Вкладка Текст
            }
            if (li.Length > 0)
            {
                sb.AppendLine("         <ul class=\"nav nav-tabs\" id=\"myTab\">");
                sb.AppendLine(li.ToString());
                sb.AppendLine("			</ul>");
                sb.AppendLine("         <div class=\"tab-content\">");
                sb.AppendLine(tp.ToString());
                sb.AppendLine("         </div>");
            }
            sb.AppendLine("     </div>");
            sb.AppendLine("     </form>");
            js.AppendLine();
            js.AppendLine("     $('#myTab a:first').tab('show');");

            #endregion Табличная часть

            #region Футер

            // Футер
            sb.AppendLine("<div class=\"card-modal-footer\">");
            sb.AppendLine("     <div class=\"card-modal-footer-left\">");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"$('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=" + Master.SqlBase + "&curTable=" + Master.TableSql + "&curPage=" + Master.PageName + "&action=create&curId=0&_=' + (new Date()).getTime());\">Новая</button>");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\" $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=" + Master.SqlBase + "&curTable=" + Master.TableSql + "&curPage=" + Master.PageName + "&action=copy&curId=" + curId + "&_=' + (new Date()).getTime());\">Копировать</button>");
            sb.AppendLine("     </div>");
            sb.AppendLine("     <div class=\"card-modal-footer-rigth\">");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"if($('#SaveButton').is(':disabled')){$('#EditDialog').modal('hide');}else if (confirm('Закрыть без сохранения?')){$('#EditDialog').modal('hide');}\">Закрыть</button>");
            sb.AppendLine("         <button type=\"button\" id =\"SaveButton\" class=\"btn btn-primary btn-sm\" onclick=\"SubmitForm();\" disabled>Сохранить</button>");
            sb.AppendLine("     </div>");
            sb.AppendLine("</div>");
            sb.AppendLine();

            #endregion Футер

            #region Скрипты

            if (Master.Action==TableAction.Copy)
            {
                curId = "0";
            }

            sb.AppendLine("<script>");
            sb.AppendLine();
            sb.AppendLine("     $('#EditDialog input').bind('change keyup', function(event) {AllowSave();});"); // Активирует кнопку Сохранить при изменениях в инпутах
            sb.AppendLine(js.ToString());
            sb.AppendLine();
            sb.AppendLine("     function AllowSave() {"); // Снимает блок с кнопки «Сохранить»
            sb.AppendLine("         $('#SaveButton').prop('disabled', false);");
            sb.AppendLine("     }");
            sb.AppendLine();
            sb.AppendLine("     function SubmitForm() {");
            sb.AppendLine("           var formData = new FormData($('#EditForm')[0]); ");
            sb.AppendLine("         $.ajax({");
            sb.AppendLine("             type: 'POST',");
            sb.AppendLine("             url: '/Handler/CardSaveDataHandler.ashx?curBase=" + Master.SqlBase + "&curTable=" + Master.TableSql + "&curPage=" + Master.PageName + "&curId=" + curId + "&action=" + Master.Action + "', ");
            sb.AppendLine("             data: formData,");
            sb.AppendLine("             processData: false,");
            sb.AppendLine("             async: false,");
            sb.AppendLine("             cache: false,");
            sb.AppendLine("             contentType: false,");
            sb.AppendLine("             success: function(data) {");
            sb.AppendLine("                $('.card-input-error').html('&nbsp;');");// убираем предыдущие сообщения об ошибках
            sb.AppendLine("                $('.card-input-group').removeClass('has-error');"); // убираем красный цвет
            sb.AppendLine("                if (data.indexOf('fieldErrors')!=-1){");
            sb.AppendLine("                     var jdata = JSON.parse(data);");
            sb.AppendLine("                     jdata.fieldErrors.forEach(function(item, i, arr) {");
            sb.AppendLine("                         $('#'+item.name+'Error').html(item.status);");
            sb.AppendLine("                         $('#'+item.name).parent().addClass('has-error');");
            sb.AppendLine("                     });");
            sb.AppendLine("                }");
            sb.AppendLine("                else if (data.indexOf('error')!=-1){");
            sb.AppendLine("                     var jdata = JSON.parse(data);");
            sb.AppendLine("                     $('#HeaderError').hide();");
            sb.AppendLine("                     $('#HeaderError').html(jdata.error);");
            sb.AppendLine("                     $('#HeaderError').fadeIn();");
            sb.AppendLine("                }");
            sb.AppendLine("                else{");
            sb.AppendLine("                     $('#EditDialogContent').load(");
            sb.AppendLine("                         '/Handler/EditDialogHandler.ashx?curBase=" + Master.SqlBase + "&curTable=" + Master.TableSql + "&curPage=" + Master.PageName + "&action=edit&_=' + (new Date()).getTime() + '&curId=" + (curId != "0" ? curId + "'" : "' + data") + ",null,");
            sb.AppendLine("                         function(){");
            sb.AppendLine("                             $('#HeaderMsg').hide();");
            sb.AppendLine("                             $('#HeaderMsg').html('" + (curId != "0" ? "Запись сохранена" : "Запись создана") + "');");
            sb.AppendLine("                             $('#HeaderMsg').fadeIn();");
            sb.AppendLine("                             setTimeout('$(\"#HeaderMsg\").fadeOut();', 3000);");
            sb.AppendLine("                         }");
            sb.AppendLine("                     );");
            sb.AppendLine("                     ");
            sb.AppendLine("                }");
            sb.AppendLine("             },");
            sb.AppendLine("             error: function(xhr, str){alert('Возникла ошибка: ' + xhr.responseCode);}");
            sb.AppendLine("         }); ");
            sb.AppendLine("     } ");
            sb.AppendLine("</script>");

            #endregion Скрипты

            return sb.ToString();
        }

        // HTML| JS общие для всей страницы
        public string GenerateContent()
        {
            StringBuilder sb = new StringBuilder();

            // Таблица HTML для шапки
            sb.AppendLine(Master.GenerateHtmlTable());

            // Карточка
            sb.AppendLine("    <div id=\"EditDialog\" class=\"modal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myLargeModalLabel\">");
            sb.AppendLine("         <div class=\"modal-dialog modal-lg\" " + (EditDialogWidth > 0 ? "style=\"width: " + EditDialogWidth + "px;\"" : "") + " role=\"document\">");
            sb.AppendLine("                 <div id=\"EditDialogContent\" class=\"modal-content\">");
            sb.AppendLine("                 </div>");
            sb.AppendLine("         </div>");
            sb.AppendLine("    </div>");

            // Фильтр HTML для шапки
            sb.AppendLine(Master.GenerateFilterFormDialog());

            // Блок JS
            sb.AppendLine("    <script>");
            sb.AppendLine("        var editor;"); // Глобальная переменная для editor'a
            sb.AppendLine(GenerateJSWindowsResize());
            sb.AppendLine("        $(document).ready(function () {");
            sb.AppendLine("            document.title = '" + BrowserTabTitle + "';");
            sb.AppendLine("            $('#curPageTitle').text('" + Master.PageTitle + "');");
            sb.AppendLine(Master.GenerateJSEditorInit());
            sb.AppendLine(Master.GenerateJSDataTable());
            sb.AppendLine(MouseDoubleClick());
            sb.AppendLine(ContextMenu());
            sb.AppendLine("            $(window).resize();");

            sb.AppendLine("        });");
            sb.AppendLine("    </script>");

            return sb.ToString();
        }

        // Обрабатывает запросы на редактирование данных
        public string Process()
        {
            // Ответ
            string result = string.Empty;

            // AJAX|JSON
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            //------
            // Шапка
            //------

            // Чекаем на простые условия (обязательность, длинна и вообще изменилось ли что нибудь)
            if (Master.Action != TableAction.Remove)
                result = Master.Validate();

            // Чекаем на условия конкретной таблицы
            if (string.IsNullOrEmpty(result))
                result = Master.Check();

            //------
            // Курсоры
            //------
            if (Detail != null && string.IsNullOrEmpty(result))
                foreach (TableData tableData in Detail)
                {
                    // Чекаем на простые условия (обязательность, длинна и вообще изменилось ли что нибудь)
                    if (string.IsNullOrEmpty(result) && tableData.Action != TableAction.Remove)
                        result = tableData.Validate();

                    // Чекаем на условия конкретной таблицы
                    if (string.IsNullOrEmpty(result))
                        result = tableData.Check();
                }

            // Сохраняем
            if (string.IsNullOrEmpty(result))
                result = Master.Save();

            return result;
        }
    }
}