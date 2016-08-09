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
    public class TableArchive : Table
    {
        public TableArchive()
        {
            ShowRowInfoButton = true;
            FieldList = new List<Field>()
            {
                new Field { Caption = "Код ЭА",             Name = "Id",            Align = Align.Left,     Width = 70 },
                new Field { Caption = "Дата редак.",        Name = "DateUpd",       Align = Align.Center,   Width = 115},
                new Field { Caption = "Оператор",           Name = "User",          Align = Align.Left,     Width = 125},
                new Field { Caption = "Номер документа",    Name = "DocNum",        Align = Align.Left,     Width = 300},
                new Field { Caption = "Дата докум.",        Name = "DocDate",       Align = Align.Center,   Width = 85},
                new Field { Caption = "Документ",           Name = "DocTree",       Align = Align.Left,     Width = 150},
                new Field { Caption = "Контрагент",         Name = "FrmContr",      Align = Align.Left,     Width = 250},
                new Field { Caption = "Содержание",         Name = "DocContent",    Align = Align.Left,     Width = 300},
                new Field { Caption = "Сумма",              Name = "Summ",          Align = Align.Right,    Width = 100},
                new Field { Caption = "Пакет",              Name = "DocPack",       Align = Align.Right,    Width = 80},
                new Field { Caption = "Примечание",         Name = "Prim",          Align = Align.Left,     Width = 300},
                new Field { Caption = "Договор",            Name = "Parent",        Align = Align.Left,     Width = 150},
                new Field { Caption = "Штрихкод",           Name = "Barcode",       Align = Align.Left,     Width = 80},
                new Field { Caption = "Статус",             Name = "Status",        Align = Align.Left,     Width = 150},
                new Field { Caption = "Источник",           Name = "Source",        Align = Align.Left,     Width = 150},
                new Field { Caption = "Дата перед.",        Name = "DateTrans",     Align = Align.Center,   Width = 85},
            };
        }
    }

    public partial class Archive : System.Web.UI.Page
    {
        public string curTable = "Archive";
        public string curPageName = string.Empty;
        public Table tableArchive = new TableArchive();

        #region Generate

        // Генерит условие из установленного фильтра
        public static string GenerateWhereClause(string curBase, string curTable, string curPage)
        {
            StringBuilder sbWhere = new StringBuilder();
            Dictionary<string, string> filterList = (Dictionary<string, string>)HttpContext.Current.Session[curBase + curTable + curPage + "UserFilterList"];
            if (filterList != null)
            {
                string key = string.Empty;
                string value = string.Empty;
                foreach (KeyValuePair<string, string> pair in filterList)
                {
                    key = pair.Key.Substring(1); // убираем вначале "f"
                    value = pair.Value;
                    switch (key)
                    {
                        case "DocNum":
                        case "DocContent":
                        case "Prim":
                            sbWhere.AppendLine(" AND a.[" + key + "] LIKE '%" + value.Replace("'", "''").Replace("[", "[[]") + "%'");
                            break;

                        case "Id":
                        case "IdUser":
                        case "IdStatus":
                        case "IdSource":
                        case "IdFrmContr":
                        case "IdDocTree":
                        case "IdParent":
                        case "Barcode":
                            sbWhere.AppendLine("    AND a.[" + key + "] = " + value);
                            break;

                        case "DateTrans":
                        case "DocDate":
                            sbWhere.AppendLine("    AND a.[DocDate] = CONVERT(DATE,'" + value + "',104)");
                            break;

                        case "DocDateBegin":
                            sbWhere.AppendLine("    AND a.[DocDate] >=CONVERT(DATE,'" + value + "',104)");
                            break;

                        case "DocDateEnd":
                            sbWhere.AppendLine("    AND a.[DocDate] <=CONVERT(DATE,'" + value + "',104)");
                            break;
                    }
                }
            }

            return sbWhere.ToString();
        }

        // Генерит содержимое под кнопкой «+»
        public static string GenerateJSTableInfoButtonContent(string curBase, string curTable, string Id)
        {
            StringBuilder li = new StringBuilder(); // Вкладки
            StringBuilder tp = new StringBuilder(); // Содержимое

            li.AppendLine("			  <li><a data-target=\"#FilesTab" + Id + "\" class=\"active\" data-toggle=\"tab\">Файлы</a></li>");
            tp.AppendLine("           <div role=\"tabpanel\" class=\"tab-pane fade\" id=\"FilesTab" + Id + "\" style=\"height: 200px;width: 500px;\">");
            tp.AppendLine("                 <div style=\"width: 470px;display: table;margin-top:10px;\">");

            DataTable dtFiles = GetFileList(curBase, curTable, "0", Id); // Получаем список файлов
            if (dtFiles.Rows.Count > 0)
            {
                string fn, fid, hash = string.Empty;
                bool isPrivate = false;
                foreach (DataRow row in dtFiles.Rows)
                {
                    fid = row["IdFile"].ToString();
                    fn = row["fileName"].ToString().Trim();
                    hash = Func.GetMd5Hash(Func.GetMd5Hash(fid) + fid);
                    isPrivate = (bool)row["IsPrivate"];

                    tp.AppendLine("                  <div class=\"file-button\"  id=\"FileButton" + fid + "\">");
                    tp.AppendLine("                         <button type=\"button\"  class=\"btn btn-default " + (isPrivate ? "opacity" : "") + "\" style=\"width:200px;\" title=\"" + fn + "\"");
                    tp.AppendLine("                              onclick =\"window.open('/Handler/GetFileHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&IdFile=" + row["IdFile"].ToString() + "&key=" + hash + "');\" >" + (fn.Length > 24 ? fn.Substring(0, 22) + ".." : fn) + "</button>");//
                    tp.AppendLine("                  </div>");
                }
            }

            tp.AppendLine("                 </div>");
            tp.AppendLine("           </div>");

            li.AppendLine("			  <li><a data-target=\"#DocTextTab" + Id + "\" data-toggle=\"tab\">Текст документа</a></li>");
            tp.AppendLine("           <div role=\"tabpanel\" class=\"tab-pane fade\" id=\"DocTextTab" + Id + "\" style=\"height: 200px;width: 500px;\">");
            tp.AppendLine("                 <textarea id=\"DocText" + Id + "\" name=\"DocText" + Id + "\" class=\"card-form-control card-textarea\">" + GetText(curBase, curTable, Id) + "</textarea>");
            tp.AppendLine("           </div>");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("     <div>");
            sb.AppendLine("         <ul class=\"nav nav-tabs\" id=\"myTab" + Id + "\">");
            sb.AppendLine(li.ToString());
            sb.AppendLine("			</ul>");
            sb.AppendLine("         <div class=\"tab-content\">");
            sb.AppendLine(tp.ToString());
            sb.AppendLine("         </div>");
            sb.AppendLine("     </div>");

            return sb.ToString();
        }

        // Генерит Диалоговое окно - карточку
        public static string GenerateEditDialog(string curBase, string curTable, string curPage, Action action, string curId)
        {
            DataRow data = null;
            if (action == Action.Edit || action == Action.Copy)
            {
                DataTable dt = GetData(curBase, curTable, curPage, 0, 1, "Id", "Asc", curId);
                if (dt.Rows.Count > 0)
                    data = dt.Rows[0];
                else return "Not found id = " + curId;
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder js = new StringBuilder();

            #region Шапка Диалога

            sb.AppendLine("<div class=\"card-modal-header\">");
            sb.AppendLine("     <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>");
            sb.AppendLine("     <div id=\"HeaderMsg\" class=\"label label-success card-modal-header-msg\"></div>");
            sb.AppendLine("     <div id=\"HeaderError\" class=\"label label-danger card-modal-header-msg\"></div>");
            switch (action)
            {
                case Action.Create:
                case Action.Copy:
                    sb.AppendLine("     <h4 class=\"modal-title\">Новая запись</h4>");
                    break;

                case Action.Edit:
                case Action.Remove:
                    sb.AppendLine("     <h4 class=\"modal-title\">Запись № " + curId.ToString() + "</h4>");
                    sb.AppendLine("     <h6 class=\"modal-title\">Дата редактирования: " + data["DateUpd"].ToString() + " " + data["User"].ToString() + "</h6>");
                    break;

                default:
                    break;
            }
            sb.AppendLine("</div>");

            #endregion Шапка Диалога

            #region Шапка документа

            string value = string.Empty;
            string valueText = string.Empty;

            sb.AppendLine("<form method=\"POST\" id=\"EditForm\" name=\"EditForm\" action=\"javascript: void(null);\" enctype=\"multipart/form-data\">");
            sb.AppendLine("<div id=\"EditDialogBody\" class=\"modal-body\">");
            sb.AppendLine("     <div class=\"row\">");

            // Номер документа
            value = (action != Action.Create ? data["DocNum"].ToString() : string.Empty);
            sb.AppendLine("         <div class=\"card-input-group\">");
            sb.AppendLine("             <label class=\"card-label\">Номер документа</label>");
            sb.AppendLine("                 <input id=\"DocNum\" name=\"DocNum\" class=\"card-form-control\" value=\"" + value + "\" >");
            sb.AppendLine("                 <div id=\"DocNumError\" class=\"card-input-error\">&nbsp;</div>");
            sb.AppendLine("         </div>");

            // Документ
            value = (action != Action.Create ? data["IdDocTree"].ToString() : "0");
            valueText = (action != Action.Create ? data["DocTree"].ToString() : string.Empty);
            sb.AppendLine("         <div id=\"scrollable-dropdown-menu\">");
            sb.AppendLine("             <div class=\"card-input-group\">");
            sb.AppendLine("                 <label class=\"card-label\">Документ</label>");
            sb.AppendLine("                 <input type=\"text\"  id=\"DocTree\" onchange=\"if ($('#DocTree').val().trim() == '')$('#IdDocTree').val(0);\" ");
            sb.AppendLine("                     class=\"card-form-control\"  value=\"" + valueText + "\" placeholder=\"Начните вводить для поиска по справочнику..\">");
            sb.AppendLine("                 <input type=\"hidden\" id=\"IdDocTree\" name=\"IdDocTree\" value=\"" + value + "\">");
            sb.AppendLine("                 <div id=\"IdDocTreeError\" class=\"card-input-error\">&nbsp;</div>");
            sb.AppendLine("             </div>");
            sb.AppendLine("         </div>");
            js.AppendLine("         var sourceDocTree = new Bloodhound({");
            js.AppendLine("                datumTokenizer: Bloodhound.tokenizers.whitespace,");
            js.AppendLine("                queryTokenizer: Bloodhound.tokenizers.whitespace,");
            js.AppendLine("                remote: {");
            js.AppendLine("                    url: '/Handler/TypeaheadHandler.ashx?t=DocTree&q=%QUERY',");
            js.AppendLine("                    wildcard: '%QUERY'");
            js.AppendLine("                },");
            js.AppendLine("                limit: 30,");
            js.AppendLine("         });");
            js.AppendLine();
            js.AppendLine("         $('#scrollable-dropdown-menu #DocTree').typeahead({");
            js.AppendLine("                highlight: true,");
            js.AppendLine("                minLength: 1,");
            js.AppendLine("         },");
            js.AppendLine("         {");
            js.AppendLine("                name: 'thDocTree',");
            js.AppendLine("                display: 'Name',");
            js.AppendLine("                highlight: true,");
            js.AppendLine("                limit: 30,");
            js.AppendLine("                source: sourceDocTree,");
            js.AppendLine("         });");
            js.AppendLine();
            js.AppendLine("         $(\"#DocTree\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#IdDocTree\").val(datum.ID); });");

            // Дата документа
            value = (action != Action.Create ? data["DocDate"].ToString() : string.Empty);
            sb.AppendLine("         <div class=\"card-input-group\">");
            sb.AppendLine("             <label class=\"card-label\" >Дата документа</label>");
            sb.AppendLine("             <input id=\"DocDate\" name=\"DocDate\" class=\"card-form-control\" value=\"" + value + "\" >");
            sb.AppendLine("             <div id=\"DocDateError\" class=\"card-input-error\">&nbsp;</div>");
            sb.AppendLine("         </div>");
            js.AppendLine("         $('#DocDate').mask('99.99.9999',{ placeholder: 'дд.мм.гггг'}); ");
            js.AppendLine("         $('#DocDate').datetimepicker({locale: 'ru', useCurrent:false, format: 'DD.MM.YYYY',}); ");

            // Содержание
            value = (action != Action.Create ? data["DocContent"].ToString() : string.Empty);
            sb.AppendLine("         <div class=\"card-input-group\">");
            sb.AppendLine("             <label class=\"card-label\">Содержание</label>");
            sb.AppendLine("                 <input id=\"DocContent\" name=\"DocContent\" class=\"card-form-control\" value=\"" + value + "\" >");
            sb.AppendLine("                 <div id=\"DocContentError\" class=\"card-input-error\">&nbsp;</div>");
            sb.AppendLine("         </div>");

            // Контрагент
            value = (action != Action.Create ? data["IdFrmContr"].ToString() : "0");
            valueText = (action != Action.Create ? data["FrmContr"].ToString() : string.Empty);
            sb.AppendLine("         <div id=\"scrollable-dropdown-menu\">");
            sb.AppendLine("             <div class=\"card-input-group\">");
            sb.AppendLine("                 <label class=\"card-label\" >Контрагент</label>");
            sb.AppendLine("                 <input type=\"text\"  id=\"FrmContr\" onchange=\"if ($('#FrmContr').val().trim() == '')$('#IdFrmContr').val(0);\" ");
            sb.AppendLine("                     class=\"card-form-control\"  value=\"" + valueText + "\" placeholder=\"Начните вводить для поиска по справочнику..\">");
            sb.AppendLine("                 <input type=\"hidden\" id=\"IdFrmContr\" name=\"IdFrmContr\" value=\"" + value + "\">");
            sb.AppendLine("                 <div id=\"IdFrmContrError\" class=\"card-input-error\">&nbsp;</div>");
            sb.AppendLine("             </div>");
            sb.AppendLine("         </div>");
            js.AppendLine("         var sourceFrmContr = new Bloodhound({");
            js.AppendLine("                datumTokenizer: Bloodhound.tokenizers.whitespace,");
            js.AppendLine("                queryTokenizer: Bloodhound.tokenizers.whitespace,");
            js.AppendLine("                remote: {");
            js.AppendLine("                    url: '/Handler/TypeaheadHandler.ashx?t=Frm&q=%QUERY',");
            js.AppendLine("                    wildcard: '%QUERY'");
            js.AppendLine("                },");
            js.AppendLine("                limit: 30,");
            js.AppendLine("         });");
            js.AppendLine();
            js.AppendLine("         $('#scrollable-dropdown-menu #FrmContr').typeahead({");
            js.AppendLine("                highlight: true,");
            js.AppendLine("                minLength: 1,");
            js.AppendLine("         },");
            js.AppendLine("         {");
            js.AppendLine("                name: 'thFrmContr',");
            js.AppendLine("                display: 'Name',");
            js.AppendLine("                highlight: true,");
            js.AppendLine("                limit: 30,");
            js.AppendLine("                source: sourceFrmContr,");
            js.AppendLine("         });");
            js.AppendLine();
            js.AppendLine("         $(\"#FrmContr\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#IdFrmContr\").val(datum.ID); });");

            // Сумма
            value = (action != Action.Create ? data["Summ"].ToString() : "0");
            sb.AppendLine("             <div class=\"card-input-group\">");
            sb.AppendLine("                 <label class=\"card-label\" >Сумма</label>");
            sb.AppendLine("                 <input id=\"Summ\" name=\"Summ\" class=\"card-form-control\" value=\"" + value + "\" >");
            sb.AppendLine("                 <div id=\"SummError\" class=\"card-input-error\">&nbsp;</div>");
            sb.AppendLine("             </div>");
            js.AppendLine();
            js.AppendLine("             $('#Summ').val(accounting.formatNumber($('#Summ').val().trim().replace(',', '.'), 2, ' '));");
            js.AppendLine("             $('#Summ').bind('blur', function(event) {");
            js.AppendLine("                 this.value = accounting.formatNumber(this.value.trim().replace(',', '.'), 2, ' ');");
            js.AppendLine("             });");
            js.AppendLine("             $('#Summ').bind('focus', function(event) {");
            js.AppendLine("                 this.value = accounting.unformat(this.value.trim());");
            js.AppendLine("             });");

            // Пакет
            value = (action != Action.Create ? data["DocPack"].ToString() : "0");
            sb.AppendLine("         <div class=\"card-input-group\">");
            sb.AppendLine("             <label class=\"card-label\">Пакет</label>");
            sb.AppendLine("                 <input id=\"DocPack\" name=\"DocPack\" class=\"card-form-control\" value=\"" + value + "\" >");
            sb.AppendLine("                 <div id=\"DocPackError\" class=\"card-input-error\">&nbsp;</div>");
            sb.AppendLine("         </div>");

            // Примечание
            value = (action != Action.Create ? data["Prim"].ToString() : string.Empty);
            sb.AppendLine("         <div class=\"card-input-group\">");
            sb.AppendLine("             <label class=\"card-label\">Примечание</label>");
            sb.AppendLine("                 <input id=\"Prim\" name=\"Prim\" class=\"card-form-control\" value=\"" + value + "\" >");
            sb.AppendLine("                 <div id=\"PrimError\" class=\"card-input-error\">&nbsp;</div>");
            sb.AppendLine("         </div>");
            sb.AppendLine("     </div>");// row

            #endregion Шапка документа

            #region Файлы

            sb.AppendLine("     <div>");
            StringBuilder li = new StringBuilder(); // Вкладки
            StringBuilder tp = new StringBuilder(); // Содержимое

            li.AppendLine("			  <li><a data-target=\"#FilesTab\" data-toggle=\"tab\">Файлы</a></li>");
            tp.AppendLine("           <div role=\"tabpanel\" class=\"tab-pane fade\" id=\"FilesTab\" style=\"height: 300px;\">");
            tp.AppendLine("                 <label class=\"btn btn-primary btn-file\">");
            tp.AppendLine("                     Добавить&nbsp;<span id=\"badge\" class=\"badge\"></span><input id=\"Files\" name=\"Files\" type=\"file\" multiple/ onchange=\"$('#badge').html('Файлов:'+$('#Files').get(0).files.length);\">");
            tp.AppendLine("                 </label>");
            tp.AppendLine("                 <div style=\"width: 470px;display: table;margin-top:10px;\">");
            tp.AppendLine("                     <input type=\"hidden\" id=\"FilesToPrivate\" name=\"FilesToPrivate\" value=\"0\">");
            tp.AppendLine("                     <input type=\"hidden\" id=\"FilesToDelete\" name=\"FilesToDelete\" value=\"0\">");

            if (action != Action.Create && action != Action.Copy) // Если карточка не новая | скопированная
            {
                DataTable dtFiles = GetFileList(curBase, curTable, data["IdVer"].ToString()); // Получаем список файлов
                if (dtFiles.Rows.Count > 0)
                {
                    string fn, fid, hash = string.Empty;
                    bool isPrivate = false;
                    foreach (DataRow row in dtFiles.Rows)
                    {
                        fid = row["IdFile"].ToString();
                        fn = row["fileName"].ToString().Trim();
                        hash = Func.GetMd5Hash(Func.GetMd5Hash(fid) + fid);
                        isPrivate = (bool)row["IsPrivate"];

                        tp.AppendLine("                  <div class=\"file-button\"  id=\"EditFileButton" + fid + "\">");
                        tp.AppendLine("                     <div class=\"btn-group\">");
                        tp.AppendLine("                         <button type=\"button\"  class=\"btn btn-default " + (isPrivate ? "opacity" : "") + "\" style=\"width:200px;\" title=\"" + fn + "\"");
                        tp.AppendLine("                              onclick =\"window.open('/Handler/GetFileHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&IdFile=" + row["IdFile"].ToString() + "&key=" + hash + "');\" >" + (fn.Length > 24 ? fn.Substring(0, 22) + ".." : fn) + "</button>");//
                        tp.AppendLine("                         <button type=\"button\" class=\"btn btn-default dropdown-toggle " + (isPrivate ? "opacity" : "") + "\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">");
                        tp.AppendLine("                             <span class=\"caret\"></span>");
                        tp.AppendLine("                             <span class=\"sr-only\">Toggle Dropdown</span>");
                        tp.AppendLine("                         </button>");
                        tp.AppendLine("                         <ul class=\"dropdown-menu\">");
                        tp.AppendLine("                             <li><a href=\"#\" onclick=\"var tp = $('#FilesToPrivate'); tp.val(tp.val()+'," + fid + "');$('#EditFileButton" + fid + " .btn').animate({opacity: 0.5}); AllowSave();\">Скрыть</a></li>");
                        tp.AppendLine("                              <li><a href=\"#\" onclick=\"var tp = $('#FilesToDelete'); tp.val(tp.val()+'," + fid + "');$('#EditFileButton" + fid + "').fadeOut();AllowSave();\">Удалить</a></li>");
                        tp.AppendLine("                         </ul>");
                        tp.AppendLine("                     </div>");
                        tp.AppendLine("                  </div>");
                    }
                }
            }
            tp.AppendLine("                 </div>");
            tp.AppendLine("           </div>");

            #endregion Файлы

            #region Текст документа

            li.AppendLine("			  <li><a data-target=\"#DocTextTab\" data-toggle=\"tab\">Текст документа</a></li>");
            tp.AppendLine("           <div role=\"tabpanel\" class=\"tab-pane fade\" id=\"DocTextTab\" style=\"height: 300px;\">");
            string text = string.Empty;
            if (action != Action.Create && action != Action.Copy) // Если карточка не новая
                text = GetText(curBase, curTable, data["Id"].ToString()); // Получаем текст для карточки
            tp.AppendLine("                 <textarea id=\"DocText\" name=\"DocText\" class=\"card-form-control card-textarea\">" + text + "</textarea>");
            tp.AppendLine("           </div>");

            #endregion Текст документа

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

            #region Футер

            // Футер
            sb.AppendLine("<div class=\"card-modal-footer\">");
            sb.AppendLine("     <div class=\"card-modal-footer-left\">");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"$('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&curPage=" + curPage + "&action=create&curId=0&_=' + (new Date()).getTime());\">Новая</button>");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\" $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?&curBase=" + curBase + "&curTable=" + curTable + "&curPage=" + curPage + "&action=copy&curId=" + curId + "&_=' + (new Date()).getTime());\">Копировать</button>");
            sb.AppendLine("     </div>");
            sb.AppendLine("     <div class=\"card-modal-footer-rigth\">");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"if($('#SaveButton').is(':disabled')){$('#EditDialog').modal('hide');}else if (confirm('Закрыть без сохранения?')){$('#EditDialog').modal('hide');}\">Закрыть</button>");
            sb.AppendLine("         <button type=\"button\" id =\"SaveButton\" class=\"btn btn-primary btn-sm\" onclick=\"SubmitForm();\" disabled>Сохранить</button>");
            sb.AppendLine("     </div>");
            sb.AppendLine("</div>");
            sb.AppendLine();

            #endregion Футер

            #region Скрипты

            if (action == Action.Copy)
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
            sb.AppendLine("             url: '/Handler/CardSaveDataHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&curPage=" + curPage + "&curId=" + curId + "&action=" + action + "', ");
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
            sb.AppendLine("                         '/Handler/EditDialogHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&curPage=" + curPage + "&action=edit&_=' + (new Date()).getTime() + '&curId=" + (curId != "0" ? curId + "'" : "' + data") + ",null,");
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

        #endregion Generate

        #region Get

        public static string GetColumnNameByIndex(int index)
        {
            Table t = new TableArchive();
            if (t.ShowRowInfoButton) index--;
            return t.FieldList[index].Name;
        }

        // Возвращает список файлов для текущей версии  idVer - id версии
        public static DataTable GetFileList(string curBase, string curTable, string idVer, string Id = "0")
        {
            // Запрос
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("SELECT T.IdFile, F.fileName, F.IsPrivate ");
            if (Id == "0")
            {
                sbQuery.AppendLine("FROM [dbo].[" + curBase + curTable + "FileList] T");
                sbQuery.AppendLine("JOIN [dbo].[" + curBase + curTable + "Files] F ON F.Id = T.IdFile");
                sbQuery.AppendLine("WHERE T.IdVer = " + idVer);
            }
            else
            {
                sbQuery.AppendLine("FROM [dbo].[" + curBase + curTable + "] A");
                sbQuery.AppendLine("JOIN [dbo].[" + curBase + curTable + "FileList] T ON T.IdVer=A.IdVer");
                sbQuery.AppendLine("JOIN [dbo].[" + curBase + curTable + "Files] F ON F.Id = T.IdFile");
                sbQuery.AppendLine("WHERE A.Active=1 AND A.Del=0 AND A.Id = " + Id);
            }

            DataTable dt = Func.GetData(sbQuery.ToString());
            return dt;
        }

        // Возвращает текст текущей версии id - id Карточки
        public static string GetText(string curBase, string curTable, string id)
        {
            // Запрос
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("SELECT [Text] ");
            sbQuery.AppendLine("FROM [dbo].[" + curBase + curTable + "Text]");
            sbQuery.AppendLine("WHERE IdArchive = " + id);

            // Выполняем запрос
            var res = Func.ExecuteScalar(sbQuery.ToString());
            if (res is DBNull || res == null)
                return string.Empty;
            else
                return res.ToString();
        }

        public static DataTable GetData(string curBase, string curTable, string curPage, int displayStart, int displayLength, string sortCol, string sortDir, string ids = null)
        {
            // Запрос
            StringBuilder sbQuery = new StringBuilder();
            // Условия отборки
            StringBuilder sbWhere = new StringBuilder();

            sbWhere.AppendLine("	a.Del=0 ");
            sbWhere.AppendLine("	AND a.Active=1 ");
            sbWhere.AppendLine(GenerateWhereClause(curBase, curTable, curPage));

            if (!string.IsNullOrEmpty(ids))
                sbWhere.AppendLine("    AND a.Id in (" + ids + ")");

            //
            sbQuery.AppendLine("DECLARE @recordsFiltered int;");
            sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
            sbQuery.AppendLine("FROM [dbo].[" + curBase + curTable + "] a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine(sbWhere.ToString());
            sbQuery.AppendLine(";");

            sbQuery.AppendLine("SELECT * FROM  (");
            sbQuery.AppendLine("   SELECT @recordsFiltered AS recordsFiltered");
            sbQuery.AppendLine("   ,T.Id");
            sbQuery.AppendLine("   ,T.IdVer");
            sbQuery.AppendLine("   ,T.Active");
            sbQuery.AppendLine("   ,T.Del");
            sbQuery.AppendLine("   ,T.DateUpd");
            sbQuery.AppendLine("   ,T.IdUser");
            sbQuery.AppendLine("   ,U.Name as [User]");
            //
            sbQuery.AppendLine("   ,T.DocNum");
            sbQuery.AppendLine("   ,T.DocDate");
            sbQuery.AppendLine("   ,T.IdDocTree");
            sbQuery.AppendLine("   ,DT2.Name as DocTree");
            sbQuery.AppendLine("   ,T.Prim");
            sbQuery.AppendLine("   ,T.DocContent");
            sbQuery.AppendLine("   ,T.IdFrmContr");
            sbQuery.AppendLine("   ,F.Name as FrmContr");
            sbQuery.AppendLine("   ,T.Summ");
            sbQuery.AppendLine("   ,T.DocPack");
            sbQuery.AppendLine("   ,T.IdParent");
            sbQuery.AppendLine("   ,P.DocNum as [Parent]");
            sbQuery.AppendLine("   ,T.Barcode");
            sbQuery.AppendLine("   ,T.IdStatus");
            sbQuery.AppendLine("   ,ST.Name as Status");
            sbQuery.AppendLine("   ,T.IdSource");
            sbQuery.AppendLine("   ,SR.Name as Source");
            sbQuery.AppendLine("   ,T.DateTrans");
            sbQuery.AppendLine("   FROM [dbo].[" + curBase + curTable + "] T");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[Frm] F on T.IdFrmContr = F.Id");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] U on T.IdUser = U.Id");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[Status] ST on T.IdStatus = ST.Id");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[Source] SR on T.IdSource = SR.Id");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[DocTree] DT2 on T.IdDocTree = DT2.Id");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[" + curBase + curTable + "] P on T.IdParent = P.Id AND P.Active=1 AND P.Del=0");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine(sbWhere.ToString());
            sbQuery.AppendLine("ORDER BY a.[" + sortCol + "] " + sortDir);
            sbQuery.AppendLine("OFFSET @displayStart ROWS FETCH FIRST @displayLength ROWS ONLY");

            SqlParameter[] sqlParameterArray = {
                new SqlParameter { ParameterName = "@displayStart", SqlDbType = SqlDbType.Int, Value = displayStart },
                new SqlParameter { ParameterName = "@displayLength", SqlDbType = SqlDbType.Int, Value = displayLength }
            };

            DataTable dt = Func.GetData(sbQuery.ToString(), sqlParameterArray);
            return dt;
        }

        // Форматирует полученные после запроса данные
        private static List<Dictionary<string, object>> GetFormatData(DataTable dt)
        {
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            CultureInfo ruRu = CultureInfo.CreateSpecificCulture("ru-RU");
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    switch (dc.ColumnName)
                    {
                        case "Summ":
                            row.Add(dc.ColumnName, String.Format(ruRu, "{0:0,0.00}", Convert.ToDecimal(dr[dc.ColumnName])));
                            break;

                        case "DateUpd":
                            if (dr[dc.ColumnName] is DBNull)
                                row.Add(dc.ColumnName, string.Empty);
                            else
                                row.Add(dc.ColumnName, ((DateTime)dr[dc.ColumnName]).ToString("dd.MM.yyyy HH:mm:ss"));
                            break;

                        case "DocDate":
                        case "DateTrans":
                            if (dr[dc.ColumnName] is DBNull)
                                row.Add(dc.ColumnName, string.Empty);
                            else
                                row.Add(dc.ColumnName, ((DateTime)dr[dc.ColumnName]).ToString("dd.MM.yyyy"));
                            break;

                        default:
                            row.Add(dc.ColumnName, dr[dc.ColumnName].ToString());
                            break;
                    }
                }
                data.Add(row);
            }
            return data;
        }

        // Формирует ответ гриду в JSON
        public static string GetJsonData(string curBase, string curTable, DataTable dt, int drawCount)
        {
            string ret = string.Empty;

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            if (dt != null)
            {
                var result = new
                {
                    draw = drawCount,
                    recordsTotal = (int)Func.ExecuteScalar("SELECT COUNT(*) FROM [dbo].[" + curBase + curTable + "] WHERE Del=0 AND Active=1"),
                    recordsFiltered = Convert.ToInt32(dt.Rows.Count > 0 ? dt.Rows[0]["recordsFiltered"] : 0),
                    data = GetFormatData(dt)
                };
                ret = javaScriptSerializer.Serialize(result);
            }
            else 
            {
                ret = javaScriptSerializer.Serialize(new { error = "Данные не получены" });
            }
            return ret;
        }

        #endregion Get

        #region Process

        public static string Validate(Dictionary<string, List<RequestData>> requestRows)
        {
            // Флаг для прерывания/продолжения проверки
            bool resume = true;

            // Список ошибок
            List<FieldErrors> fieldErrors = new List<FieldErrors>();

            // Проверяем каждую пару, для первой или единственной строки
            if (resume)
            {
                foreach (RequestData rd in requestRows.First().Value)
                {
                    resume = true;

                    // Обязательность заполнения поля
                    switch (rd.FieldName)
                    {
                        case "DocNum":
                            // Для текстовых/числовых не пропускаем пустые строки, для справочников не пропускаем еще и нули
                            if (string.IsNullOrEmpty(rd.FieldValue))
                            {
                                fieldErrors.Add(new FieldErrors { name = rd.FieldName, status = "Поле обязательно для заполнения" });
                                resume = false;
                            }
                            break;
                        // Для справочников не пропускаем еще и нули
                        case "IdFrmContr":

                            if (string.IsNullOrEmpty(rd.FieldValue) || rd.FieldValue == "0")
                            {
                                fieldErrors.Add(new FieldErrors { name = rd.FieldName, status = "Поле обязательно для заполнения" });
                                resume = false;
                            }
                            break;
                    }

                    // Проверяем тип введенных данных
                    if (resume)
                    {
                        switch (rd.FieldName)
                        {
                            case "DocDate":
                                if (!string.IsNullOrEmpty(rd.FieldValue))
                                {
                                    DateTime date;
                                    if (!DateTime.TryParse(rd.FieldValue, out date))
                                    {
                                        fieldErrors.Add(new FieldErrors { name = rd.FieldName, status = "Неверный формат даты" });
                                        resume = false;
                                    }
                                    else if (date > new DateTime(2020, 1, 1) || date < new DateTime(2000, 1, 1))
                                    {
                                        fieldErrors.Add(new FieldErrors { name = rd.FieldName, status = "Выбрана недопустимая дата" });
                                        resume = false;
                                    }
                                    else rd.FieldValue = date.ToString("yyyy-MM-dd");
                                }
                                break;

                            case "Summ":
                                decimal money;
                                rd.FieldValue = rd.FieldValue.Replace('.', ',');
                                if (!decimal.TryParse(rd.FieldValue, out money))
                                {
                                    fieldErrors.Add(new FieldErrors { name = rd.FieldName, status = "Неверный формат суммы" });
                                    resume = false;
                                }
                                break;
                        }
                    }

                    // Ограничения
                    if (resume)
                    {
                        switch (rd.FieldName)
                        {
                            case "DocNum":
                            case "DocContent":
                            case "Prim":
                                if (rd.FieldValue.Length > 250)
                                {
                                    fieldErrors.Add(new FieldErrors { name = rd.FieldName, status = "Максимально допустимая длина поля: 250 симв." });
                                    resume = false;
                                }
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

        // Сохраняет изменения в базе
        public static string Save(string curBase, string curTable, string curPage, Action tableAction, Dictionary<string, List<RequestData>> requestRows, HttpFileCollection requestFiles)
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
                switch (tableAction)
                {
                    case Action.Create:
                    case Action.Copy:
                        // Для каждой переданной строки с данными, создаем строку запроса и параметры к ней, выполняем запрос
                        foreach (KeyValuePair<string, List<RequestData>> pair in requestRows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            // Обновляем запись в главной таблице
                            query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "]");
                            query.AppendLine("    ([DateUpd]"); // Дата внесения
                            query.AppendLine("    ,[IdUser]"); // Пользователь внесший изменения
                            query.AppendLine("    ,[DocNum]");
                            query.AppendLine("    ,[DocDate]");
                            query.AppendLine("    ,[Prim]");
                            query.AppendLine("    ,[DocContent]");
                            query.AppendLine("    ,[IdFrmContr]");
                            query.AppendLine("    ,[IdDocTree]");
                            query.AppendLine("    ,[IdDocType]");
                            query.AppendLine("    ,[Summ]");
                            query.AppendLine("    ,[DocPack]");
                            query.AppendLine("    )");
                            query.AppendLine("VALUES ");
                            query.AppendLine("    (GetDate()");
                            query.AppendLine("    ,@IdUser");
                            query.AppendLine("    ,@DocNum");
                            query.AppendLine("    ,@DocDate");
                            query.AppendLine("    ,@Prim");
                            query.AppendLine("    ,@DocContent");
                            query.AppendLine("    ,@IdFrmContr");
                            query.AppendLine("    ,@IdDocTree");
                            query.AppendLine("    ,@IdDocType");
                            query.AppendLine("    ,@Summ");
                            query.AppendLine("    ,@DocPack");
                            query.AppendLine("    );");

                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = HttpContext.Current.Session["UserId"].ToString() });
                            object value = null;
                            foreach (RequestData rd in pair.Value)
                            {
                                switch (rd.FieldName)
                                {
                                    case "DocNum":
                                    case "Prim":
                                    case "DocContent":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.NVarChar, Value = rd.FieldValue });
                                        break;

                                    case "IdFrmContr":
                                    case "IdDocTree":
                                    case "IdDocType":
                                    case "DocPack":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Int, Value = rd.FieldValue });
                                        break;

                                    case "Summ":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Decimal, Value = rd.FieldValue });
                                        break;

                                    case "DocDate":
                                        if (string.IsNullOrEmpty(rd.FieldValue))
                                            value = DBNull.Value;
                                        else
                                            value = rd.FieldValue;
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Date, Value = value });
                                        break;
                                }
                            }

                            query.AppendLine();
                            query.AppendLine("DECLARE @si AS int;");
                            query.AppendLine("SET @si = SCOPE_IDENTITY();");
                            query.AppendLine("UPDATE[dbo].[" + curBase + curTable + "] SET[Id] = [IdVer] WHERE IdVer = @si;");// Записываем новый Id=IdVer
                            query.AppendLine();

                            // Сохраняем файлы
                            if (requestFiles != null)
                            {
                                query.AppendLine("DECLARE @fileId AS int;");
                                for (int i = 0; i < requestFiles.Count; i++)
                                {
                                    HttpPostedFile file = requestFiles[i];

                                    if (file.ContentLength > 0)
                                    {
                                        byte[] fileData = null;
                                        using (var binaryReader = new BinaryReader(file.InputStream))
                                        {
                                            fileData = binaryReader.ReadBytes(file.ContentLength);
                                        }
                                        query.AppendLine();
                                        query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "Files]([fileDATA],[fileName])VALUES(@fileData" + i + ",@fileName" + i + ");");// Записали файл
                                        query.AppendLine("SET @fileId = SCOPE_IDENTITY();");// Узнали его id
                                        query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "FileList]([IdVer],[IdFile])VALUES(@si,@fileId);");

                                        param.Add(new SqlParameter { ParameterName = "@fileName" + i, SqlDbType = SqlDbType.NVarChar, Value = Path.GetFileName(file.FileName).Trim() });
                                        param.Add(new SqlParameter { ParameterName = "@fileData" + i, SqlDbType = SqlDbType.VarBinary, Value = fileData });
                                    }
                                }
                            }

                            // Записываем текст документа
                            string text = pair.Value.Find(x => x.FieldName == "DocText").FieldValue;
                            query.AppendLine();
                            query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "Text]([IdArchive],[Text])VALUES(@si,@Text);");
                            param.Add(new SqlParameter { ParameterName = "@Text", SqlDbType = SqlDbType.Text, Value = text });

                            query.AppendLine("SELECT @si;");

                            sqlCommand = new SqlCommand(query.ToString(), sqlConnection, sqlTransaction);
                            sqlCommand.Parameters.AddRange(param.ToArray());
                            result = sqlCommand.ExecuteScalar().ToString(); // Получаем Id новой записи
                        }
                        break;

                    case Action.Edit:
                        // Для каждой переданной строки с данными, создаем строку запроса и параметры к ней, выполняем запрос
                        foreach (KeyValuePair<string, List<RequestData>> pair in requestRows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            query.AppendLine("DECLARE @nextVer AS int;");
                            query.AppendLine("DECLARE @prevVer AS int;");

                            // Номер текущей версии
                            query.AppendLine("SELECT @prevVer = IdVer FROM [dbo].[" + curBase + curTable + "] WHERE Id = @Id AND [Active]=1;");

                            // Снимаем активность предыдущих записей
                            query.AppendLine("UPDATE[dbo].[" + curBase + curTable + "] SET [Active] = 0 WHERE Id = @Id AND [Active]=1;");
                            query.AppendLine();

                            // Добавляем новую версию
                            query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "]");
                            query.AppendLine("    ([Id]"); // Дата внесения
                            query.AppendLine("    ,[DateUpd]"); // Дата внесения
                            query.AppendLine("    ,[IdUser]"); // Пользователь внесший изменения
                            query.AppendLine("    ,[DocNum]");
                            query.AppendLine("    ,[DocDate]");
                            query.AppendLine("    ,[Prim]");
                            query.AppendLine("    ,[DocContent]");
                            query.AppendLine("    ,[IdFrmContr]");
                            query.AppendLine("    ,[IdDocTree]");
                            query.AppendLine("    ,[IdDocType]");
                            query.AppendLine("    ,[Summ]");
                            query.AppendLine("    ,[DocPack]");
                            query.AppendLine("    )");
                            query.AppendLine("VALUES ");
                            query.AppendLine("    (@Id");
                            query.AppendLine("    ,GetDate()");
                            query.AppendLine("    ,@IdUser");
                            query.AppendLine("    ,@DocNum");
                            query.AppendLine("    ,@DocDate");
                            query.AppendLine("    ,@Prim");
                            query.AppendLine("    ,@DocContent");
                            query.AppendLine("    ,@IdFrmContr");
                            query.AppendLine("    ,@IdDocTree");
                            query.AppendLine("    ,@IdDocType");
                            query.AppendLine("    ,@Summ");
                            query.AppendLine("    ,@DocPack");
                            query.AppendLine("    );");

                            param.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = pair.Key });
                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = HttpContext.Current.Session["UserId"].ToString() });
                            object value = null;
                            foreach (RequestData rd in pair.Value)
                            {
                                switch (rd.FieldName)
                                {
                                    case "DocNum":
                                    case "Prim":
                                    case "DocContent":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.NVarChar, Value = rd.FieldValue });
                                        break;

                                    case "IdFrmContr":
                                    case "IdDocTree":
                                    case "IdDocType":
                                    case "DocPack":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Int, Value = rd.FieldValue });
                                        break;

                                    case "Summ":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Decimal, Value = rd.FieldValue });
                                        break;

                                    case "DocDate":
                                        if (string.IsNullOrEmpty(rd.FieldValue))
                                            value = DBNull.Value;
                                        else
                                            value = rd.FieldValue;
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Date, Value = value });
                                        break;
                                }
                            }

                            query.AppendLine();
                            query.AppendLine("SET @nextVer = SCOPE_IDENTITY();"); // Узнали номер следующей версии

                            // Сохраняем файлы
                            if (requestFiles != null)
                            {
                                query.AppendLine("DECLARE @fileId AS int;");
                                for (int i = 0; i < requestFiles.Count; i++)
                                {
                                    HttpPostedFile file = requestFiles[i];
                                    if (file.ContentLength > 0)
                                    {
                                        byte[] fileData = null;
                                        using (var binaryReader = new BinaryReader(file.InputStream))
                                        {
                                            fileData = binaryReader.ReadBytes(file.ContentLength);
                                        }
                                        query.AppendLine();
                                        query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "Files]([fileDATA],[fileName])VALUES(@fileData" + i + ",@fileName" + i + ");");// Записали файл
                                        query.AppendLine("SET @fileId = SCOPE_IDENTITY();");// Узнали его id
                                        query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "FileList]([IdVer],[IdFile])VALUES(@nextVer,@fileId);");

                                        param.Add(new SqlParameter { ParameterName = "@fileName" + i, SqlDbType = SqlDbType.NVarChar, Value = Path.GetFileName(file.FileName) });
                                        param.Add(new SqlParameter { ParameterName = "@fileData" + i, SqlDbType = SqlDbType.VarBinary, Value = fileData });
                                    }
                                }
                            }

                            // Скрываем файлы
                            string toPrivate = requestRows[pair.Key].Find(x => x.FieldName == "FilesToPrivate").FieldValue;
                            if (!string.IsNullOrEmpty(toPrivate))
                            {
                                query.AppendLine("UPDATE [dbo].[" + curBase + curTable + "Files] SET [IsPrivate] = 1 WHERE [ID] in (" + toPrivate + ");");
                            }
                            query.AppendLine();

                            // Копируем файлы из предыдущей версии без удаленных
                            string toDelete = requestRows[pair.Key].Find(x => x.FieldName == "FilesToDelete").FieldValue;
                            query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "FileList]([IdVer],[IdFile]) ");
                            query.AppendLine("      SELECT @nextVer, IdFile FROM [dbo].[" + curBase + curTable + "FileList]");
                            query.AppendLine("      WHERE IdVer=@prevVer" + (string.IsNullOrEmpty(toDelete) ? "" : " AND [IdFile] NOT IN (" + toDelete + ")") + ";");

                            // Сохраняем текст
                            string text = pair.Value.Find(x => x.FieldName == "DocText").FieldValue;
                            query.AppendLine();
                            query.AppendLine("UPDATE [dbo].[" + curBase + curTable + "Text] SET [Text]=@Text WHERE [IdArchive]=" + pair.Key + " and CAST([Text] as nvarchar(max))<>@Text");
                            param.Add(new SqlParameter { ParameterName = "@Text", SqlDbType = SqlDbType.NVarChar, Value = text });

                            sqlCommand = new SqlCommand(query.ToString(), sqlConnection, sqlTransaction);
                            sqlCommand.Parameters.AddRange(param.ToArray());
                            sqlCommand.ExecuteNonQuery();
                        }
                        break;

                    case Action.Remove:// TODO : удалять из основной и версий совсем устаревшие данные (полгода), routine
                        foreach (KeyValuePair<string, List<RequestData>> pair in requestRows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            // Снимаем активность предыдущих записей
                            query.AppendLine("UPDATE[dbo].[" + curBase + curTable + "] SET [Active] = 0, [Del] = 1 WHERE Id = @Id;");
                            query.AppendLine();

                            // Добавляем новую версию
                            query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "]");
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

                Func.LogSqlError(ex, sqlCommand.CommandText, param.ToArray());
            }
            finally
            {
                sqlConnection.Close();
            }

            return result;
        }

        // Обрабатывает запросы на редактирование данных
        public static string Process(string curBase, string curTable, string curPage, Action tableAction, Dictionary<string, List<RequestData>> requestRows, HttpFileCollection requestFiles)
        {
            // Ответ
            string result = string.Empty;

            // AJAX|JSON
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            //------
            // Шапка
            //------

            // Чекаем на простые условия (обязательность, длинна и вообще изменилось ли что нибудь)
            if (tableAction != Action.Remove)
                result = Validate(requestRows);

            //------
            // Курсоры
            //------
            //if (Detail != null && string.IsNullOrEmpty(result))
            //    foreach (TableData tableData in Detail)
            //    {
            //        // Чекаем на простые условия (обязательность, длинна и вообще изменилось ли что нибудь)
            //        if (string.IsNullOrEmpty(result) && tableData.Action != TableAction.Remove)
            //            result = tableData.Validate();

            //        // Чекаем на условия конкретной таблицы
            //        if (string.IsNullOrEmpty(result))
            //            result = tableData.Check();
            //    }

            // Сохраняем
            if (string.IsNullOrEmpty(result))
                result = Save(curBase, curTable, curPage, tableAction, requestRows, requestFiles);

            return result;
        }

        #endregion Process

        protected void Page_PreRender(object sender, EventArgs e)
        {
            curPageName = Func.GetArchivePageNameRus(Master.curPage);
        }
    }
}