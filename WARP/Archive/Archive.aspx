<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="WARP.Archive" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">

    <table id="table" class="table table-striped table-bordered table-condensed" style="table-layout: fixed; width: 100%">
        <thead>
            <tr>
                <th></th>
                <th>Код ЭА</th>
                <th>Дата редак.</th>
                <th>Оператор</th>
                <th>Номер документа</th>
                <th>Документ</th>
                <th>Вид документа</th>
                <th>Дата докум.</th>
                <th>Содержание</th>
                <th>Контрагент</th>
                <th>Сумма</th>
                <th>Пакет</th>
                <th>Примечание</th>
            </tr>
        </thead>
    </table>

    <div id="EditDialog" class="modal" tabindex="-1" role="dialog" aria-labelledby="myLargeModalLabel">
        <div class="modal-dialog modal-lg" style="width: 1007px;" role="document">
            <div id="EditDialogContent" class="modal-content">
            </div>
        </div>
    </div>

    <script>
        var editor;

        $(window).bind('resize', function () {
            $('.dataTables_scrollBody').css('height', ($(window).height() - 125) + 'px');
        });

        $(document).ready(function () {

            document.title = 'Общие';

            $('#curPageTitle').text('Электронный архив | База: ООО «НТЗ» | Документы | Все');

            editor = new $.fn.dataTable.Editor({
                ajax: "/Handler/GridSaveDataHandler.ashx?curBase=Ntz&curTable=Archive&curPage=All",
                table: "#table",
                idSrc: 'Id',
                fields: [

                         {
                             label: "Номер документа:",
                             name: "DocNum",
                         },
                         {
                             label: "Документ:",
                             name: "DocTree",
                         },
                         {
                             label: "Вид документа:",
                             name: "DocType",
                         },
                         {
                             label: "Дата документа:",
                             name: "DocDate",
                         },
                         {
                             label: "Содержание:",
                             name: "DocContent",
                         },
                         {
                             label: "Контрагент:",
                             name: "FrmContr",
                         },
                         {
                             label: "Сумма:",
                             name: "Summ",
                         },
                         {
                             label: "Примечание:",
                             name: "Prim",
                         },
                         

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
                        restore: "Отменить"
                    },
                }
            });

            var table = $('#table').DataTable({
                dom: '<"row top-toolbar"<"col-sm-4"B><"col-sm-4"><"col-sm-4"pi>><"row top-toolbar">Zrt',
                rowId: 'Id',
                processing: true,
                serverSide: true,
                ajax: "/Handler/GetDataHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&curPage=<%=Master.curPage%>",
                columns: [
                    { "className": 'details-control', "orderable": false, "data": null, "defaultContent": '', "width": "20px" },
                    { "data": "Id", className: "dt-body-left", "width": "70px" },
                    { "data": "DateUpd", className: "dt-body-center", "width": "115px" },
                    { "data": "User", className: "dt-body-left", "width": "125px" },
                    { "data": "DocNum", className: "dt-body-left", "width": "300px" },
                    { "data": "DocTree", className: "dt-body-left", "width": "150px" },
                    { "data": "DocType", className: "dt-body-left", "width": "150px" },
                    { "data": "DocDate", className: "dt-body-center", "width": "85px" },
                    { "data": "DocContent", className: "dt-body-left", "width": "300px" },
                    { "data": "FrmContr", className: "dt-body-left", "width": "250px" },
                    { "data": "Summ", className: "dt-body-right", "width": "100px" },
                    { "data": "DocPack", className: "dt-body-right", "width": "100px" },
                    { "data": "Prim", className: "dt-body-left", "width": "300px" },

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
                lengthMenu: [[30, 100, 200, 500], ['30 строк', '100 строк', '200 строк', '500 строк']],
                pagingType6: "simple",
                buttons: [
                    {
                        text: '<span class="glyphicon glyphicon-plus" title="Создать новую запись"></span>',
                        className: 'btn-sm',
                        action: function (e, dt, node, config) {
                            $('#EditDialog').modal();
                            $('#EditDialogContent').html('Загрузка..');
                            $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&curPage=<%=Master.curPage%>&action=create&curId=0&_=' + (new Date()).getTime());
                        },
                        key: "n",
                        className: "btn-sm",
                    },
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
                                extend: 'collection',
                                text: 'Выбрать представление',
                                buttons: [
                                    {
                                        text: 'Общие',
                                        action: function (e, dt, node, config) {
                                            dt.state.clear();
                                            window.location.reload();
                                        },
                                    },
                                    {
                                        text: 'Бухгалтерские документы',
                                        action: function (e, dt, node, config) {
                                            dt.state.clear();
                                            window.location.reload();
                                        },
                                    },
                                    {
                                        text: 'Договоры',
                                        action: function (e, dt, node, config) {
                                            dt.state.clear();
                                            window.location.reload();
                                        },
                                    },
                                ],
                            },
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
                            },
                        ],
                        className: "btn-sm",
                    },
                    {
                        text: 'Фильтр',
                        action: function (e, dt, node, config) {
                            $('#modalFilterForm').modal();
                        },
                        key: "a",
                        className: "btn-sm",
                    }

                ],
                language: {
                    url: '/content/DataTables-1.10.12/js/Russian.json'
                }
            });

            var detailRows = [];

            function format(id) { return '<div id="RowInfo' + id + '"></div>'; }

            $('#table tbody').on('click', 'tr td.details-control', function () {
                var tr = $(this).closest('tr');
                var row = table.row(tr);

                if (row.child.isShown()) {
                    tr.removeClass('details');
                    row.child.hide();
                }
                else {
                    tr.addClass('details');
                    row.child('<div id="RowInfo' + row.data().Id + '">Загрузка..</div>').show();
                    $('#RowInfo' + row.data().Id).load(
                        '/Handler/InfoButtonHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&curPage=<%=Master.curPage%>&curId=' + row.data().Id + '&_=' + (new Date()).getTime(), null,
                        function () {
                            $('#myTab'+ row.data().Id+' a:first').tab('show');
                        }
                    );
                }
            });

            $('#table').on('dblclick', 'tr', function () {
                var table = $('#table').DataTable();
                var id = table.row(this).id();
                $('#EditDialog').modal();
                $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&curPage=<%=Master.curPage%>&action=edit&curId=' + id + '&_=' + (new Date()).getTime());
            });

            $.contextMenu({
                selector: '#table .selected td',
                items: {
                    foo: {
                        name: 'Foo',
                        callback: function (key, opt) {
                            alert('Foo!');
                        }
                    },
                    bar: {
                        name: 'Bar',
                        callback: function (key, opt) {
                            alert('Bar!')
                        }
                    }
                }
            });

            $(window).resize();
        });
    </script>
</asp:Content>