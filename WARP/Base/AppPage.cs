using System.Collections.Generic;
using System.Text;

namespace WARP
{
    public class AppPage
    {
        // Текст на вкладке браузера
        public string BrowserTabTitle { get; set; } = string.Empty;

        // Ширина карточки
        public int EditDialogWidth { get; set; } = 0;

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
            sb.AppendLine("                $('#EditDialogContent').html('Загрузка..');");
            sb.AppendLine("                $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=" + Master.SqlBase + "&curTable=" + Master.TableSql + "&curPage=" + Master.PageName + "&curId=' + id);");
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

        public virtual string GenerateEditDialog(int curId)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"card-modal-header\">");
            sb.AppendLine("     <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>");
            sb.AppendLine("     <h4 class=\"modal-title\">Запись № " + curId.ToString() + "</h4>");
            sb.AppendLine("     <h6 class=\"modal-title\">Дата редактирования: 01.01.2001 01:01:01 Фазылянов АН</h6>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div id=\"EditDialogBody\" class=\"modal-body\">");
            sb.AppendLine("     <div class=\"row\">");
            foreach (TableColumn tableColumn in Master.ColumnList)
            {
                switch (tableColumn.EditType)
                {
                    case TableColumnEditType.None:
                        break;

                    case TableColumnEditType.CurrentUser:
                        break;

                    case TableColumnEditType.CurrentDateTime:
                        break;

                    case TableColumnEditType.String:
                        sb.AppendLine("             <div class=\"card-input-group\">");
                        sb.AppendLine("                 <label class=\"card-label\" >" + tableColumn.ViewCaption + "</label>");
                        sb.AppendLine("                 <input id=\"" + tableColumn.DataNameSql + "\" name=\"" + tableColumn.DataNameSql + "\" class=\"card-form-control\"  value=\"\">");
                        sb.AppendLine("             </div>");
                        break;

                    case TableColumnEditType.Autocomplete:
                        break;

                    case TableColumnEditType.Integer:
                        break;

                    case TableColumnEditType.Money:
                        break;

                    case TableColumnEditType.DropDown:
                        break;

                    default:
                        break;
                }
            }

            sb.AppendLine("     </div>");
            sb.AppendLine("     <div class=\"row\">");
            sb.AppendLine("     таб часть будет тут");
            sb.AppendLine("     </div>");
            sb.AppendLine("     <div class=\"row\">");
            sb.AppendLine("     </div>");

            sb.AppendLine("<div class=\"modal-footer\">");
            sb.AppendLine("     <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Close</button>");
            sb.AppendLine("     <button type=\"button\" class=\"btn btn-primary\">Save changes</button>");
            sb.AppendLine("</div>");

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
    }
}