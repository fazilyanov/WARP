<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="WARP.Archive" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">
    <%=tableData.GenerateHtml()%>

    
    <table id="table_id" class="table table-striped table-bordered table-condensed" style="table-layout: fixed; width: 100%">
        <thead>
            <tr>
                <%=tableData.GenerateHtmlTableColumns()%>
            </tr>
        </thead>
    </table>

    <%=tableData.GenerateFilterFormDialog()%>

    <script>

        // Для выбора вручную страницы
        //var table = $('#table_id').DataTable(); table.page(4).draw('page');
        $(window).bind('resize', function () {
            var h = $(window).height();
            $('.dataTables_scrollBody').css('height', (h - 125) + 'px');
        });

        var editor;
        $(document).ready(function () {
            document.title = '<%=browserTabTitle%>';
            $('#curPageTitle').text('<%=documentTitle%>');

            editor = new $.fn.dataTable.Editor({
                ajax: "/Handler/SaveDataHandler.ashx?curBase=<%=Master.curBaseName%>&curTable=<%=tableData.TableSql%>&curPage=<%=curPage%>",
                table: "#table_id",
                idSrc: 'ID',
                fields: [
                    <%=tableData.GenerateJSEditorTableColumns()%>
                ],
                i18n: {
                    create: {
                        button: "Новая запись",
                        title: "Создание новой записи",
                        submit: "Создать"
                    },
                    edit: {
                        button: "Редактировать",
                        title: "Редактирование записи",
                        submit: "Сохранить"
                    },
                    remove: {
                        button: "Удалить",
                        title: "Удаление",
                        submit: "Подтвердить удаление",
                        confirm: {
                            _: "Подтвердите удаление %d записей?",
                            1: "Подтвердите удаление записи?"
                        }
                    },
                    error: {
                        system: "Произошла ошибка"
                    },
                    multi: {
                        title: "Множественное редактирование",
                        info: "Выбранные записи содержат разное значение для этого поля, для замены новым значением всех записей, кликните по этому полю ",
                        restore: "Отменить"
                    },
                }
            });
            //$('#table_id').on('click', 'tbody td:not(:first-child)', function (e) {
            //    editor.inline(this);
            //});

            var table = $('#table_id').DataTable({
                dom: '<"row top-toolbar"<"col-sm-4"B><"col-sm-4"p><"col-sm-4"i>>Zrt',
                processing: true,
                serverSide: true,
                ajax: "/Handler/GetDataHandler.ashx?curBase=<%=Master.curBaseName%>&curTable=<%=tableData.TableSql%>&curPage=<%=curPage%>",
                "columns": [
                    <%=tableData.GenerateJSTableColumns()%>
                ],
                autoWidth: false,
                select: true,
                colReorder: {
                    realtime: false
                },
                colResize: {
                    "tableWidthFixed": true
                },
                stateSave: true,
                scrollY: ($(window).height() - 125) + "px",
                scrollX: true,
                scrollCollapse: false,
                lengthMenu: [
                    [30, 100, 200, 500],
                    ['30 строк', '100 строк', '200 строк', '500 строк']
                ],
                pagingType6: "simple",
                buttons: [
                    { extend: 'create', editor: editor, className: 'btn-sm', key: "l", text: '<span class="glyphicon glyphicon-plus" title="Создать новую запись"></span>' },
                    { extend: 'edit', editor: editor, className: 'btn-sm', key: "h", text: '<span class="glyphicon glyphicon-pencil" title="Редактировать запись"></span>' },
                    {
                        extend: "selectedSingle",
                        className: 'btn-sm',
                        text: '<span class="glyphicon glyphicon-duplicate" title="Создать новую запись копированием текущей"></span>',
                        action: function (e, dt, node, config) {
                            var values = editor.edit(
                                    table.row({ selected: true }).index(),
                                    false
                                )
                                .val();
                            editor
                                .create({
                                    title: 'Создание копированием записи',
                                    buttons: 'Создать'
                                })
                                .set(values);
                        }
                    },
                    { extend: 'remove', editor: editor, className: 'btn-sm btn-space', key: "e", text: '<span class="glyphicon glyphicon-trash" title="Удалить текущую запись"></span>' },
                    {
                        extend: 'collection',
                        text: 'Настройка таблицы',
                        buttons: [
                            {
                                extend: 'colvis',
                                text: 'Видимость столбцов',
                                postfixButtons: ['colvisRestore']
                            },
                            {
                                extend: 'pageLength',
                                text: 'Записей на страницу'
                            },
                            {
                                text: 'Сбросить все настройки',
                                action: function (e, dt, node, config) {
                                    dt.state.clear();
                                    window.location.reload();
                                }
                            }
                        ],
                        className: "btn-sm",
                    },
                    {
                        text: 'Фильтр',
                        action: function (e, dt, node, config) {
                            $('#modalFilterForm').modal();
                        },
                        key:"a",
                        className: "btn-sm",
                    }

                ],
                language: {
                    url: '/content/DataTables-1.10.12/js/Russian.json'
                }
            });
            $(window).resize();
        });
    </script>
</asp:Content>