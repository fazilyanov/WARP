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
    public class TableComplect : Table
    {
        public TableComplect()
        {
            ShowRowInfoButton = false;
            FieldList = new List<Field>()
            {
                new Field { Caption = "Id",            Name = "Id",            Align = Align.Left,     Width = 70 },//
                new Field { Caption = "Дата созд.",    Name = "DateCreate",    Align = Align.Left,     Width = 100},//
                new Field { Caption = "Создал",        Name = "Creator",       Align = Align.Center,   Width = 115},//
                new Field { Caption = "Дата редак.",   Name = "DateUpd",       Align = Align.Left,     Width = 100},//
                new Field { Caption = "Редактировал",  Name = "Editor",        Align = Align.Center,   Width = 115},//
                new Field { Caption = "Имя",           Name = "Name",          Align = Align.Left,     Width = 100},//
                new Field { Caption = "Получатель",    Name = "Perf",          Align = Align.Left,     Width = 100},//
                new Field { Caption = "Кол-во док.",   Name = "DocCount",      Align = Align.Left,     Width = 100},//
                new Field { Caption = "Примечание",    Name = "Prim",          Align = Align.Left,     Width = 100},//
            };
        }
    }

    public class TableComplectDetail : Table
    {
        public TableComplectDetail()
        {
            ShowRowInfoButton = false;
            FieldList = new List<Field>()
            {
                new Field { Caption = "Id",            Name = "Id",            Align = Align.Left,     Width = 70 },//
                new Field { Caption = "Дата созд.",    Name = "DateCreate",    Align = Align.Left,     Width = 100},//
                new Field { Caption = "Создал",        Name = "Creator",       Align = Align.Center,   Width = 115},//
                new Field { Caption = "Дата редак.",   Name = "DateUpd",       Align = Align.Left,     Width = 100},//
                new Field { Caption = "Редактировал",  Name = "Editor",        Align = Align.Center,   Width = 115},//
                new Field { Caption = "Штихкод",       Name = "Barcode",       Align = Align.Left,     Width = 100},//
            };
        }
    }

    public partial class Complect : System.Web.UI.Page
    {
        public string curTable = "Complect";
        public Table tableComplect = new TableComplect();

        #region Generate

        // Генерит условие из установленного фильтра
        public static string GenerateWhereClause(string curBase, string curTable)
        {
            StringBuilder sbWhere = new StringBuilder();
            Dictionary<string, string> filterList = (Dictionary<string, string>)HttpContext.Current.Session[curBase + curTable + "UserFilterList"];
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
                        case "Name":
                        case "Prim":
                            sbWhere.AppendLine(" AND a.[" + key + "] LIKE '%" + value.Replace("'", "''").Replace("[", "[[]") + "%'");
                            break;

                        case "Id":
                        case "IdCreator":
                        case "IdEditor":
                        case "IdPerf":
                        case "Barcode":
                        case "DocCount":
                            sbWhere.AppendLine("    AND a.[" + key + "] = " + value);
                            break;

                        case "DateUpdBegin":
                        case "DateCreateBegin":
                            sbWhere.AppendLine("    AND a.[" + key.Remove(key.Length - 5) + "] >=CONVERT(DATETIME,'" + value + "',104)");
                            break;

                        case "DateUpdEnd":
                        case "DateCreateEnd":
                            sbWhere.AppendLine("    AND a.[" + key.Remove(key.Length - 3) + "] <=CONVERT(DATETIME,'" + value + "',104)");
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
        public static string GenerateEditDialog(string curBase, string curTable, Action action, string curId)
        {
            // Достаем значения полей шапки
            DataRow data = null;
            if (action == Action.Edit || action == Action.Copy)
            {
                DataTable dt = GetData(curBase, curTable, 0, 1, "Id", "Asc", curId);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                        data = dt.Rows[0];
                    else return Lit.ID_NOT_FOUND + curId;
                }
                else return Lit.ERROR_GET_DATA;
            }

            //Подготавливаем табчасть
            StringBuilder q = new StringBuilder();

            // Чистим временную таблицу, от временных записей пользователя
            q.AppendLine("DELETE FROM [dbo].[" + curBase + curTable + "DetailTemp] WHERE IdComplect = " + curId + " AND IdUser = " + HttpContext.Current.Session["UserId"].ToString() + ";");

            // Если открыли на редактирование, копируем всю табчасть во временную таблицу (без удаленных)
            if (action == Action.Edit)
            {
                q.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "DetailTemp]");
                q.AppendLine("  SELECT " + HttpContext.Current.Session["UserId"].ToString() + ", 0, * "); // Пользователь, статус, остальные поля
                q.AppendLine("  FROM [dbo].[" + curBase + curTable + "Detail] WHERE IdComplect = " + curId + " and del = 0");
            }
            Db.ExecuteNonQuery(q.ToString());

            //
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
                    sb.AppendLine("     <h6 class=\"modal-title\">Дата редактирования: " + data["DateUpd"].ToString() + " " + data["Editor"].ToString() + " Дата создания: " + data["DateCreate"].ToString() + " " + data["Creator"].ToString() + "</h6>");
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
            sb.AppendLine("     <div class=\"row\" style=\"padding-left:5px;\">");

            // Имя
            value = (action != Action.Create ? data["Name"].ToString() : string.Empty);
            sb.AppendLine("         <div class=\"card-input-group\">");
            sb.AppendLine("             <label class=\"card-label\">Имя</label>");
            sb.AppendLine("                 <input id=\"Name\" name=\"Name\" class=\"card-form-control\" value=\"" + value + "\" >");
            sb.AppendLine("                 <div id=\"NameError\" class=\"card-input-error\"></div>");
            sb.AppendLine("         </div>");

            // Получатель
            value = (action != Action.Create ? data["IdPerf"].ToString() : "0");
            valueText = (action != Action.Create ? data["Perf"].ToString() : string.Empty);
            sb.AppendLine("         <div id=\"scrollable-dropdown-menu\">");
            sb.AppendLine("             <div class=\"card-input-group\">");
            sb.AppendLine("                 <label class=\"card-label\">Получатель</label>");
            sb.AppendLine("                 <input type=\"text\"  id=\"Perf\" onchange=\"if ($('#Perf').val().trim() == '')$('#IdPerf').val(0);\" ");
            sb.AppendLine("                     class=\"card-form-control\"  value=\"" + valueText + "\" placeholder=\"Начните вводить для поиска по справочнику..\">");
            sb.AppendLine("                 <input type=\"hidden\" id=\"IdPerf\" name=\"IdPerf\" value=\"" + value + "\">");
            sb.AppendLine("                 <div id=\"IdPerfError\" class=\"card-input-error\"></div>");
            sb.AppendLine("             </div>");
            sb.AppendLine("         </div>");
            js.AppendLine("         var sourcePerf = new Bloodhound({");
            js.AppendLine("                datumTokenizer: Bloodhound.tokenizers.whitespace,");
            js.AppendLine("                queryTokenizer: Bloodhound.tokenizers.whitespace,");
            js.AppendLine("                remote: {");
            js.AppendLine("                    url: '/Handler/TypeaheadHandler.ashx?t=User&q=%QUERY',");
            js.AppendLine("                    wildcard: '%QUERY'");
            js.AppendLine("                },");
            js.AppendLine("                limit: 30,");
            js.AppendLine("         });");
            js.AppendLine();
            js.AppendLine("         $('#scrollable-dropdown-menu #Perf').typeahead({");
            js.AppendLine("                highlight: true,");
            js.AppendLine("                minLength: 1,");
            js.AppendLine("         },");
            js.AppendLine("         {");
            js.AppendLine("                name: 'thPerf',");
            js.AppendLine("                display: 'Name',");
            js.AppendLine("                highlight: true,");
            js.AppendLine("                limit: 30,");
            js.AppendLine("                source: sourcePerf,");
            js.AppendLine("         });");
            js.AppendLine();
            js.AppendLine("         $(\"#Perf\").on(\"typeahead:selected typeahead:autocompleted\", function (e, datum) { $(\"#IdPerf\").val(datum.ID); });");

            // Примечание
            value = (action != Action.Create ? data["Prim"].ToString() : string.Empty);
            sb.AppendLine("         <div class=\"card-input-group\">");
            sb.AppendLine("             <label class=\"card-label\">Примечание</label>");
            sb.AppendLine("                 <input id=\"Prim\" name=\"Prim\" class=\"card-form-control\" value=\"" + value + "\" >");
            sb.AppendLine("                 <div id=\"PrimError\" class=\"card-input-error\"></div>");
            sb.AppendLine("         </div>");
            sb.AppendLine("     </div>");// row

            #endregion Шапка документа

            #region Табличная часть

            sb.AppendLine("     <div style=\"padding-top:13px;\">");
            StringBuilder li = new StringBuilder(); // Вкладки
            StringBuilder tp = new StringBuilder(); // Содержимое

            Table tableComplectDetail = new TableComplectDetail();
            li.AppendLine("			  <li><a data-target=\"#Detail\" data-toggle=\"tab\">Документы</a></li>");
            tp.AppendLine("           <div role=\"tabpanel\" class=\"tab-pane fade\" id=\"Detail\" style=\"height: 400px;\">");
            tp.AppendLine("                 <table id=\"tableDetail\" class=\"table table-striped table-bordered table-condensed\" style=\"table-layout: fixed; width:100%\">");
            tp.AppendLine("                     <thead>");
            tp.AppendLine("                         <tr>");
            tp.AppendLine(tableComplectDetail.GenerateHtmlTable());
            tp.AppendLine("                         </tr>");
            tp.AppendLine("                     </thead>");
            tp.AppendLine("                 </table>");
            tp.AppendLine("           </div>");
            js.AppendLine("           tableDetail = $('#tableDetail').DataTable({");
            js.AppendLine("                dom: '<\"row top-toolbar\"<\"col-sm-7\"B><\"col-sm-5\"<\"row\"<\"col-sm-7\"><\"col-sm-5\"i>>>><\"top-filterbar\">Zrt',");
            js.AppendLine("                rowId: 'Id',");
            js.AppendLine("                processing: true,");
            js.AppendLine("                serverSide: true,");
            js.AppendLine("                ajax: \"/Handler/GetDataHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "Detail" + "&curId=" + curId + "\",");
            js.AppendLine("                columns: [");
            js.AppendLine(tableComplectDetail.GenerateJSTableColumns());
            js.AppendLine("                ],");
            js.AppendLine("                autoWidth: false,");
            js.AppendLine("                select: true,");
            js.AppendLine("                colReorder: {");
            js.AppendLine("                    realtime: false");
            js.AppendLine("                },");
            js.AppendLine("                colResize: {");
            js.AppendLine("                    \"tableWidthFixed\": true");
            js.AppendLine("                },");
            js.AppendLine("                stateSave: true,");
            js.AppendLine("                scrollY: \"300px\",");
            js.AppendLine("                scrollX: true,");
            js.AppendLine("                scrollCollapse: false,");
            js.AppendLine("                lengthMenu: [[30, 100, 200, 500], ['30 строк', '100 строк', '200 строк', '500 строк']],");
            js.AppendLine("                pagingType6: \"simple\",");
            js.AppendLine("                buttons: [");
            js.AppendLine("                    {");
            js.AppendLine("                        text: '<span class=\"glyphicon glyphicon-plus\" title=\"Создать новую запись\"></span>',");
            js.AppendLine("                        className: 'btn-sm',");
            js.AppendLine("                        action: function (e, dt, node, config) {");
            js.AppendLine("                            $('#EditDialogDetail').modal();");
            js.AppendLine("                            $('#EditDialogDetailContent').html('Загрузка..');");
            js.AppendLine("                            $('#EditDialogDetailContent').load('/Handler/EditDialogHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "Detail" + "&idMaster=" + curId + "&action=create&curId=0&_=' + (new Date()).getTime());");
            js.AppendLine("                        },");
            js.AppendLine("                        key: \"n\",");
            js.AppendLine("                        className: \"btn-sm\",");
            js.AppendLine("                    },");
            js.AppendLine("                    { extend: 'remove', editor: editor, className: 'btn-sm btn-space', key: \"e\", text: '<span class=\"glyphicon glyphicon-trash\" title=\"Удалить текущую запись\"></span>' },");
            js.AppendLine("                    {");
            js.AppendLine("                        extend: 'collection',");
            js.AppendLine("                        text: 'Настройка таблицы',");
            js.AppendLine("                        buttons: [");
            js.AppendLine("                            {");
            js.AppendLine("                                extend: 'colvis',");
            js.AppendLine("                                text: 'Видимость столбцов',");
            js.AppendLine("                                postfixButtons: ['colvisRestore']");
            js.AppendLine("                            },");
            js.AppendLine("                            {");
            js.AppendLine("                                text: 'Сбросить все настройки',");
            js.AppendLine("                                action: function (e, dt, node, config) {");
            js.AppendLine("                                    dt.state.clear();");
            js.AppendLine("                                    CloseForm();$(\".dt-button-background\").trigger(\"click\");");
            js.AppendLine("                                }");
            js.AppendLine("                            },");
            js.AppendLine("                        ],");
            js.AppendLine("                        className: \"btn-sm\",");
            js.AppendLine("                    },");
            js.AppendLine("                ],");
            js.AppendLine("                createdRow:");
            js.AppendLine("                     function(row, data, dataIndex){");
            js.AppendLine("                         if (data['State'] > 0 ){");
            js.AppendLine("                             $(row).addClass('pre-save-row');");
            js.AppendLine("                         }");
            js.AppendLine("                     },");
            js.AppendLine("                language: {");
            js.AppendLine("                    url: '/content/DataTables-1.10.12/js/Russian.json'");
            js.AppendLine("                }");
            js.AppendLine("            });");
            js.AppendLine(); // ROWDBKCLK
            js.AppendLine("            $('#tableDetail').on('dblclick', 'tr', function () {");
            js.AppendLine("                var table = $('#tableDetail').DataTable();");
            js.AppendLine("                var id = table.row(this).id();");
            js.AppendLine("                if (id > 0) {");
            js.AppendLine("                    $('#EditDialogDetail').modal();");
            js.AppendLine("                    $('#EditDialogDetailContent').load('/Handler/EditDialogHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "Detail" + "&idMaster=" + curId + "&action=edit&curId=' + id + '&_=' + (new Date()).getTime());");
            js.AppendLine("                }");
            js.AppendLine("            });");

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
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"$('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&action=create&curId=0&_=' + (new Date()).getTime());\">Новая</button>");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\" $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?&curBase=" + curBase + "&curTable=" + curTable + "&action=copy&curId=" + curId + "&_=' + (new Date()).getTime());\">Копировать</button>");
            sb.AppendLine("     </div>");
            sb.AppendLine("     <div class=\"card-modal-footer-rigth\">");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" data-dismiss=\"modal\">Закрыть</button>");
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
            sb.AppendLine("     $('#EditDialog input, #EditDialog textarea').bind('change keyup', function(event) {AllowSave();});"); // Активирует кнопку Сохранить при изменениях в инпутах
            sb.AppendLine(js.ToString());
            sb.AppendLine();
            sb.AppendLine("     function AllowSave() {"); // Снимает блок с кнопки «Сохранить»
            sb.AppendLine("         $('#SaveButton').prop('disabled', false);");
            sb.AppendLine("     }");
            sb.AppendLine();
            sb.AppendLine("     $('#EditDialog').on('hide.bs.modal', function (e) {");
            sb.AppendLine("         if($('#SaveButton').is(':disabled') || confirm('Закрыть без сохранения?')){ ");
            sb.AppendLine("             $(this).off('hide.bs.modal');");
            sb.AppendLine("         }");
            sb.AppendLine("         else {");
            sb.AppendLine("             e.preventDefault();");
            sb.AppendLine("             e.stopImmediatePropagation();");
            sb.AppendLine("             return false;");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine();
            sb.AppendLine("     function SubmitForm() {");
            sb.AppendLine("           var formData = new FormData($('#EditForm')[0]); ");
            sb.AppendLine("         $.ajax({");
            sb.AppendLine("             type: 'POST',");
            sb.AppendLine("             url: '/Handler/CardSaveDataHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&curId=" + curId + "&action=" + action + "', ");
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
            sb.AppendLine("                         '/Handler/EditDialogHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&action=edit&_=' + (new Date()).getTime() + '&curId=" + (curId != "0" ? curId + "'" : "' + data") + ",null,");
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

        // Генерит Диалоговое окно для записей в табличной части
        public static string GenerateEditDialogDetail(string curBase, string curTable, Action action, string idMaster, string curId)
        {
            DataRow data = null;
            if (action == Action.Edit || action == Action.Copy)
            {
                DataTable dt = GetDataDetail(curBase, curTable, "Id", "Asc", string.Empty, curId);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                        data = dt.Rows[0];
                    else return Lit.ID_NOT_FOUND + curId;
                }
                else return Lit.ERROR_GET_DATA;
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder js = new StringBuilder();

            #region Шапка Диалога

            sb.AppendLine("<div class=\"card-modal-header\">");
            sb.AppendLine("     <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>");
            sb.AppendLine("     <div id=\"HeaderMsgDetail\" class=\"label label-success card-modal-header-msg\"></div>");
            sb.AppendLine("     <div id=\"HeaderErrorDetail\" class=\"label label-danger card-modal-header-msg\"></div>");
            switch (action)
            {
                case Action.Create:
                case Action.Copy:
                    sb.AppendLine("     <h4 class=\"modal-title\">Новая запись</h4>");
                    sb.AppendLine("     <h6 class=\"modal-title\">&nbsp;</h6>");
                    break;

                case Action.Edit:
                case Action.Remove:
                    sb.AppendLine("     <h4 class=\"modal-title\">Запись № " + curId.ToString() + "</h4>");
                    sb.AppendLine("     <h6 class=\"modal-title\">Дата редактирования: " + data["DateUpd"].ToString() + " " + data["Editor"].ToString() + " Дата создания: " + data["DateCreate"].ToString() + " " + data["Creator"].ToString() + "</h6>");
                    break;

                default:
                    break;
            }
            sb.AppendLine("</div>");

            #endregion Шапка Диалога

            #region Шапка документа

            string value = string.Empty;
            string valueText = string.Empty;

            sb.AppendLine("<form method=\"POST\" id=\"EditFormDetail\" name=\"EditFormDetail\" action=\"javascript: void(null);\" enctype=\"multipart/form-data\">");
            sb.AppendLine("<div class=\"modal-body\">");
            sb.AppendLine("     <div class=\"row\" style=\"padding-left:5px;\">");

            // Штрихкод
            value = (action != Action.Create ? data["Barcode"].ToString() : "0");
            sb.AppendLine("         <div class=\"card-input-group\">");
            sb.AppendLine("             <label class=\"card-label\">Штрихкод</label>");
            sb.AppendLine("                 <input id=\"Barcode\" name=\"Barcode\" class=\"card-form-control\" value=\"" + value + "\" >");
            sb.AppendLine("                 <div id=\"BarcodeError\" class=\"card-input-error\"></div>");
            sb.AppendLine("         </div>");

            // Файл
            sb.AppendLine("         <div class=\"card-input-group\">");
            sb.AppendLine("             <label class=\"btn btn-primary btn-file\">");
            sb.AppendLine("                 Добавить&nbsp;<span id=\"badge\" class=\"badge\"></span><input id=\"Files\" name=\"Files\" type=\"file\"/ onchange=\"$('#badge').html('Файлов:'+$('#Files').get(0).files.length);\">");
            sb.AppendLine("             </label>");
            sb.AppendLine("         </div>");

            //
            sb.AppendLine("     </div>");// row

            #endregion Шапка документа

            #region Футер

            // Футер
            sb.AppendLine("<div class=\"card-modal-footer\">");
            sb.AppendLine("     <div class=\"card-modal-footer-left\">");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\"$('#EditDialogDetailContent').load('/Handler/EditDialogHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&idMaster=" + idMaster + "&action=create&curId=0&_=' + (new Date()).getTime());\">Новая</button>");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" onclick=\" $('#EditDialogDetailContent').load('/Handler/EditDialogHandler.ashx?&curBase=" + curBase + "&curTable=" + curTable + "&idMaster=" + idMaster + "&action=copy&curId=" + curId + "&_=' + (new Date()).getTime());\">Копировать</button>");
            sb.AppendLine("     </div>");
            sb.AppendLine("     <div class=\"card-modal-footer-rigth\">");
            sb.AppendLine("         <button type=\"button\" class=\"btn btn-default btn-sm\" data-dismiss=\"modal\">Закрыть</button>");
            sb.AppendLine("         <button type=\"button\" id =\"SaveButtonDetail\" class=\"btn btn-primary btn-sm\" onclick=\"SubmitFormDetail();\" disabled>Сохранить</button>");
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
            sb.AppendLine("     $('#EditDialogDetail input').bind('change keyup', function(event) {AllowSaveDetail();});"); // Активирует кнопку Сохранить при изменениях в инпутах
            sb.AppendLine(js.ToString());
            sb.AppendLine();
            sb.AppendLine("     function AllowSaveDetail() {"); // Снимает блок с кнопки «Сохранить»
            sb.AppendLine("         $('#SaveButtonDetail').prop('disabled', false);");
            sb.AppendLine("     }");
            sb.AppendLine();
            sb.AppendLine("     $('#EditDialogDetail').on('hide.bs.modal', function (e) {");
            sb.AppendLine("         if($('#SaveButtonDetail').is(':disabled') || confirm('Закрыть без сохранения?')){ ");
            sb.AppendLine("             $(this).off('hide.bs.modal');");
            sb.AppendLine("         }");
            sb.AppendLine("         else {");
            sb.AppendLine("             e.preventDefault();");
            sb.AppendLine("             e.stopImmediatePropagation();");
            sb.AppendLine("             return false;");
            sb.AppendLine("         }");
            sb.AppendLine("     });");
            sb.AppendLine();
            sb.AppendLine("     function SubmitFormDetail() {");
            sb.AppendLine("           var formData = new FormData($('#EditFormDetail')[0]); ");
            sb.AppendLine("         $.ajax({");
            sb.AppendLine("             type: 'POST',");
            sb.AppendLine("             url: '/Handler/CardSaveDataHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&curId=" + curId + "&idMaster=" + idMaster + "&action=" + action + "', ");
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
            sb.AppendLine("                     $('#HeaderErrorDetail').hide();");
            sb.AppendLine("                     $('#HeaderErrorDetail').html(jdata.error);");
            sb.AppendLine("                     $('#HeaderErrorDetail').fadeIn();");
            sb.AppendLine("                }");
            sb.AppendLine("                else{");
            sb.AppendLine("                     $('#EditDialogDetailContent').load(");
            sb.AppendLine("                         '/Handler/EditDialogHandler.ashx?curBase=" + curBase + "&curTable=" + curTable + "&idMaster=" + idMaster + "&action=edit&_=' + (new Date()).getTime() + '&curId=" + (curId != "0" ? curId + "'" : "' + data") + ",null,");
            sb.AppendLine("                         function(){");
            sb.AppendLine("                             $('#HeaderMsgDetail').hide();");
            sb.AppendLine("                             $('#HeaderMsgDetail').html('" + (curId != "0" ? "Запись сохранена" : "Запись создана") + "');");
            sb.AppendLine("                             $('#HeaderMsgDetail').fadeIn();");
            sb.AppendLine("                             setTimeout('$(\"#HeaderMsgDetail\").fadeOut();', 3000);");
            sb.AppendLine("                             tableDetail.draw();");
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
            Table t = new TableComplect();
            if (t.ShowRowInfoButton && index > 0) index--;
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

            DataTable dt = Db.GetData(sbQuery.ToString());
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
            var res = Db.ExecuteScalar(sbQuery.ToString());
            if (res is DBNull || res == null)
                return string.Empty;
            else
                return res.ToString();
        }

        public static DataTable GetData(string curBase, string curTable, int displayStart, int displayLength, string sortCol, string sortDir, string ids = null)
        {
            // Запрос
            StringBuilder sbQuery = new StringBuilder();
            // Условия отборки
            StringBuilder sbWhere = new StringBuilder();
            // Если слетела сессия
            string IdUser = (HttpContext.Current.Session["UserId"] ?? string.Empty).ToString();
            if (string.IsNullOrEmpty(IdUser))
                return null;

            sbWhere.AppendLine("	a.Del=0 ");

            if (!string.IsNullOrEmpty(ids))
                sbWhere.AppendLine("    AND a.Id in (" + ids + ")");
            else
                sbWhere.AppendLine(GenerateWhereClause(curBase, curTable));

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
            sbQuery.AppendLine("   ,T.Name");
            sbQuery.AppendLine("   ,T.IdCreator");
            sbQuery.AppendLine("   ,CR.Name as Creator");
            sbQuery.AppendLine("   ,T.IdEditor");
            sbQuery.AppendLine("   ,E.Name as Editor");
            sbQuery.AppendLine("   ,T.DateCreate");
            sbQuery.AppendLine("   ,T.DateUpd");
            sbQuery.AppendLine("   ,T.IdPerf");
            sbQuery.AppendLine("   ,P.Name as Perf");
            sbQuery.AppendLine("   ,T.Del");
            sbQuery.AppendLine("   ,T.Prim");
            sbQuery.AppendLine("   ,T.DocCount");
            sbQuery.AppendLine("   FROM [dbo].[" + curBase + curTable + "] T");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] CR on T.IdCreator = CR.Id");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] E on T.IdEditor = E.Id");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] P on T.IdPerf = P.Id");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine(sbWhere.ToString());
            sbQuery.AppendLine("ORDER BY a.[" + sortCol + "] " + sortDir);
            sbQuery.AppendLine("OFFSET @displayStart ROWS FETCH FIRST @displayLength ROWS ONLY");

            SqlParameter[] sqlParameterArray = {
                new SqlParameter { ParameterName = "@displayStart", SqlDbType = SqlDbType.Int, Value = displayStart },
                new SqlParameter { ParameterName = "@displayLength", SqlDbType = SqlDbType.Int, Value = displayLength }
            };

            DataTable dt = Db.GetData(sbQuery.ToString(), sqlParameterArray);
            return dt;
        }

        public static DataTable GetDataDetail(string curBase, string curTable, string sortCol, string sortDir, string idMaster, string id = null)
        {
            // Запрос
            StringBuilder sbQuery = new StringBuilder();
            // Условия отборки
            StringBuilder sbWhere = new StringBuilder();
            // Если слетела сессия
            string IdUser = (HttpContext.Current.Session["UserId"] ?? string.Empty).ToString();
            if (string.IsNullOrEmpty(IdUser))
                return null;
            //
            sbWhere.AppendLine("	IdUser = " + IdUser);
            if (!string.IsNullOrEmpty(idMaster))
                sbWhere.AppendLine("	AND IdComplect = " + idMaster);
            if (!string.IsNullOrEmpty(id))
                sbWhere.AppendLine("    AND a.Id in (" + id + ")");
            //
            sbQuery.AppendLine("DECLARE @recordsFiltered int;");
            sbQuery.AppendLine("SELECT @recordsFiltered=count(*)");
            sbQuery.AppendLine("FROM [dbo].[" + curBase + curTable + "Temp] a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine(sbWhere.ToString());
            sbQuery.AppendLine(";");

            sbQuery.AppendLine("SELECT * FROM  (");
            sbQuery.AppendLine("   SELECT @recordsFiltered AS recordsFiltered");
            sbQuery.AppendLine("   ,T.IdUser");
            sbQuery.AppendLine("   ,T.State");
            sbQuery.AppendLine("   ,T.Id");
            sbQuery.AppendLine("   ,T.IdComplect");
            sbQuery.AppendLine("   ,T.IdCreator");
            sbQuery.AppendLine("   ,CR.Name as Creator");
            sbQuery.AppendLine("   ,T.IdEditor");
            sbQuery.AppendLine("   ,E.Name as Editor");
            sbQuery.AppendLine("   ,T.DateCreate");
            sbQuery.AppendLine("   ,T.DateUpd");
            sbQuery.AppendLine("   ,T.Barcode");
            sbQuery.AppendLine("   ,T.Del");
            sbQuery.AppendLine("   FROM [dbo].[" + curBase + curTable + "Temp] T");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] CR on T.IdCreator = CR.Id");
            sbQuery.AppendLine("   LEFT JOIN [dbo].[User] E on T.IdEditor = E.Id");
            sbQuery.AppendLine(") a");
            sbQuery.AppendLine("WHERE");
            sbQuery.AppendLine(sbWhere.ToString());
            sbQuery.AppendLine("ORDER BY a.[" + sortCol + "] " + sortDir);

            DataTable dt = Db.GetData(sbQuery.ToString());

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
                        case "DateCreate":
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
                    recordsTotal = Db.ExecuteScalarInt("SELECT COUNT(*) FROM [dbo].[" + curBase + curTable + "] WHERE Del=0"),
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
                            case "DateTrans":
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

        public static string ValidateDetail(Dictionary<string, List<RequestData>> requestRows)
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
                            case "DateTrans":
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

            // Проверяем сессию
            string IdUser = (HttpContext.Current.Session["UserId"] ?? string.Empty).ToString();
            if (string.IsNullOrEmpty(IdUser))
                return Lit.BAD_SESSION;

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
                            query.AppendLine("    ,[IdParent]");
                            query.AppendLine("    ,[IdStatus]");
                            query.AppendLine("    ,[IdSource]");
                            query.AppendLine("    ,[Summ]");
                            query.AppendLine("    ,[DocPack]");
                            query.AppendLine("    ,[Barcode]");
                            query.AppendLine("    ,[DateTrans]");
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
                            query.AppendLine("    ,@IdParent");
                            query.AppendLine("    ,@IdStatus");
                            query.AppendLine("    ,@IdSource");
                            query.AppendLine("    ,@Summ");
                            query.AppendLine("    ,@DocPack");
                            query.AppendLine("    ,@Barcode");
                            query.AppendLine("    ,@DateTrans");
                            query.AppendLine("    );");

                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = IdUser });
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
                                    case "IdParent":
                                    case "DocPack":
                                    case "Barcode":
                                    case "IdStatus":
                                    case "IdSource":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Int, Value = rd.FieldValue });
                                        break;

                                    case "Summ":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Decimal, Value = rd.FieldValue });
                                        break;

                                    case "DocDate":
                                    case "DateTrans":
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
                            query.AppendLine("    ,[DateTrans]");
                            query.AppendLine("    ,[Prim]");
                            query.AppendLine("    ,[DocContent]");
                            query.AppendLine("    ,[IdFrmContr]");
                            query.AppendLine("    ,[IdDocTree]");
                            query.AppendLine("    ,[IdStatus]");
                            query.AppendLine("    ,[IdSource]");
                            query.AppendLine("    ,[IdParent]");
                            query.AppendLine("    ,[Summ]");
                            query.AppendLine("    ,[DocPack]");
                            query.AppendLine("    ,[Barcode]");
                            query.AppendLine("    )");
                            query.AppendLine("VALUES ");
                            query.AppendLine("    (@Id");
                            query.AppendLine("    ,GetDate()");
                            query.AppendLine("    ,@IdUser");
                            query.AppendLine("    ,@DocNum");
                            query.AppendLine("    ,@DocDate");
                            query.AppendLine("    ,@DateTrans");
                            query.AppendLine("    ,@Prim");
                            query.AppendLine("    ,@DocContent");
                            query.AppendLine("    ,@IdFrmContr");
                            query.AppendLine("    ,@IdDocTree");
                            query.AppendLine("    ,@IdStatus");
                            query.AppendLine("    ,@IdSource");
                            query.AppendLine("    ,@IdParent");
                            query.AppendLine("    ,@Summ");
                            query.AppendLine("    ,@DocPack");
                            query.AppendLine("    ,@Barcode");
                            query.AppendLine("    );");

                            param.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = pair.Key });
                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = IdUser });
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
                                    case "IdParent":
                                    case "DocPack":
                                    case "Barcode":
                                    case "IdStatus":
                                    case "IdSource":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Int, Value = rd.FieldValue });
                                        break;

                                    case "Summ":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Decimal, Value = rd.FieldValue });
                                        break;

                                    case "DocDate":
                                    case "DateTrans":
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
                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = IdUser });
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

                Log.SqlError(ex, sqlCommand.CommandText, param.ToArray());
            }
            finally
            {
                sqlConnection.Close();
            }

            return result;
        }

        // Сохраняет изменения в базе
        public static string SaveDetail(string curBase, string curTable, string curPage, string idMaster, Action tableAction, Dictionary<string, List<RequestData>> requestRows, HttpFileCollection requestFiles)
        {
            string result = string.Empty;

            // Проверяем сессию
            string IdUser = (HttpContext.Current.Session["UserId"] ?? string.Empty).ToString();
            if (string.IsNullOrEmpty(IdUser))
                return Lit.BAD_SESSION;

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

                        // Текущий максимальный id
                        int maxID = Db.ExecuteScalarInt("SELECT MAX(id) FROM [" + curBase + curTable + "Temp] WHERE IdUser=" + IdUser + " AND IdComplect=" + idMaster);

                        // Для каждой переданной строки с данными, создаем строку запроса и параметры к ней, выполняем запрос
                        foreach (KeyValuePair<string, List<RequestData>> pair in requestRows)
                        {
                            query = new StringBuilder();
                            param = new List<SqlParameter>();

                            // Обновляем запись в главной таблице
                            query.AppendLine("INSERT INTO [dbo].[" + curBase + curTable + "Temp]");
                            query.AppendLine("    ([IdUser]");
                            query.AppendLine("    ,[State]");
                            query.AppendLine("    ,[Id]");
                            query.AppendLine("    ,[IdComplect]");
                            query.AppendLine("    ,[Barcode]");
                            query.AppendLine("    ,[IdCreator]");
                            query.AppendLine("    ,[IdEditor]");
                            query.AppendLine("    ,[DateCreate]");
                            query.AppendLine("    ,[DateUpd]");
                            query.AppendLine("    ,[IdFile]");
                            query.AppendLine("    )");
                            query.AppendLine("VALUES ");
                            query.AppendLine("    (@IdUser");
                            query.AppendLine("    ,@State");
                            query.AppendLine("    ,@Id");
                            query.AppendLine("    ,@IdComplect");
                            query.AppendLine("    ,@Barcode");
                            query.AppendLine("    ,@IdCreator");
                            query.AppendLine("    ,@IdEditor");
                            query.AppendLine("    ,GetDate()");
                            query.AppendLine("    ,GetDate()");
                            query.AppendLine("    ,0");
                            query.AppendLine("    );");

                            param.Add(new SqlParameter { ParameterName = "@IdUser", SqlDbType = SqlDbType.Int, Value = IdUser });
                            param.Add(new SqlParameter { ParameterName = "@State", SqlDbType = SqlDbType.Int, Value = 1 });
                            param.Add(new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = (++maxID) });
                            param.Add(new SqlParameter { ParameterName = "@IdComplect", SqlDbType = SqlDbType.Int, Value = idMaster });
                            param.Add(new SqlParameter { ParameterName = "@IdCreator", SqlDbType = SqlDbType.Int, Value = IdUser });
                            param.Add(new SqlParameter { ParameterName = "@IdEditor", SqlDbType = SqlDbType.Int, Value = IdUser });

                            object value = null;
                            foreach (RequestData rd in pair.Value)
                            {
                                switch (rd.FieldName)
                                {
                                    case "Barcode":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Decimal, Value = rd.FieldValue });
                                        break;

                                    case "Prim_exam":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.NVarChar, Value = rd.FieldValue });
                                        break;

                                    case "IdSource_exam":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Int, Value = rd.FieldValue });
                                        break;

                                    case "DocDate_exam":
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
                            query.AppendLine();

                            // Сохраняем файлы
                            if (requestFiles != null)
                            {
                                query.AppendLine("DECLARE @fileId AS int;");

                                HttpPostedFile file = requestFiles[0];

                                if (file.ContentLength > 0)
                                {
                                    byte[] fileData = null;
                                    using (var binaryReader = new BinaryReader(file.InputStream))
                                    {
                                        fileData = binaryReader.ReadBytes(file.ContentLength);
                                    }
                                    query.AppendLine();
                                    query.AppendLine("INSERT INTO [dbo].[TempFiles]([fileDATA],[fileName])VALUES(@fileData,@fileName);");// Записали файл во временную таблицу
                                    query.AppendLine("SET @fileId = SCOPE_IDENTITY();");// Узнали его id файла
                                    query.AppendLine("UPDATE [dbo].[" + curBase + curTable + "Temp] SET [IdFile]=@fileId WHERE IdTemp=@si;");

                                    param.Add(new SqlParameter { ParameterName = "@fileName", SqlDbType = SqlDbType.NVarChar, Value = Path.GetFileName(file.FileName).Trim() });
                                    param.Add(new SqlParameter { ParameterName = "@fileData", SqlDbType = SqlDbType.VarBinary, Value = fileData });
                                }
                            }

                            query.AppendLine("SELECT " + maxID + ";");

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
                            query.AppendLine("    ,[DateTrans]");
                            query.AppendLine("    ,[Prim]");
                            query.AppendLine("    ,[DocContent]");
                            query.AppendLine("    ,[IdFrmContr]");
                            query.AppendLine("    ,[IdDocTree]");
                            query.AppendLine("    ,[IdStatus]");
                            query.AppendLine("    ,[IdSource]");
                            query.AppendLine("    ,[IdParent]");
                            query.AppendLine("    ,[Summ]");
                            query.AppendLine("    ,[DocPack]");
                            query.AppendLine("    ,[Barcode]");
                            query.AppendLine("    )");
                            query.AppendLine("VALUES ");
                            query.AppendLine("    (@Id");
                            query.AppendLine("    ,GetDate()");
                            query.AppendLine("    ,@IdUser");
                            query.AppendLine("    ,@DocNum");
                            query.AppendLine("    ,@DocDate");
                            query.AppendLine("    ,@DateTrans");
                            query.AppendLine("    ,@Prim");
                            query.AppendLine("    ,@DocContent");
                            query.AppendLine("    ,@IdFrmContr");
                            query.AppendLine("    ,@IdDocTree");
                            query.AppendLine("    ,@IdStatus");
                            query.AppendLine("    ,@IdSource");
                            query.AppendLine("    ,@IdParent");
                            query.AppendLine("    ,@Summ");
                            query.AppendLine("    ,@DocPack");
                            query.AppendLine("    ,@Barcode");
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
                                    case "IdParent":
                                    case "DocPack":
                                    case "Barcode":
                                    case "IdStatus":
                                    case "IdSource":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Int, Value = rd.FieldValue });
                                        break;

                                    case "Summ":
                                        param.Add(new SqlParameter { ParameterName = "@" + rd.FieldName, SqlDbType = SqlDbType.Decimal, Value = rd.FieldValue });
                                        break;

                                    case "DocDate":
                                    case "DateTrans":
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

                Log.SqlError(ex, sqlCommand.CommandText, param.ToArray());
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

            // Чекаем на простые условия (обязательность, длинна и вообще изменилось ли что нибудь)
            if (tableAction != Action.Remove)
                result = Validate(requestRows);

            // Сохраняем
            if (string.IsNullOrEmpty(result))
                result = Save(curBase, curTable, curPage, tableAction, requestRows, requestFiles);

            return result;
        }

        // Обрабатывает запросы на редактирование данных
        public static string ProcessDetail(string curBase, string curTable, string curPage, string idMaster, Action tableAction, Dictionary<string, List<RequestData>> requestRows, HttpFileCollection requestFiles)
        {
            // Ответ
            string result = string.Empty;

            // AJAX|JSON
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

            // Чекаем на простые условия (обязательность, длинна и вообще изменилось ли что нибудь)
            if (tableAction != Action.Remove)
                result = ValidateDetail(requestRows);

            // Сохраняем
            if (string.IsNullOrEmpty(result))
                result = SaveDetail(curBase, curTable, curPage, idMaster, tableAction, requestRows, requestFiles);

            return result;
        }

        #endregion Process

        protected void Page_PreRender(object sender, EventArgs e)
        {
        }
    }
}