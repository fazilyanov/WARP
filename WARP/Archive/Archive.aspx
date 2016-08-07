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

    <div id="extFilterDialog" class="modal" tabindex="-1" role="dialog">
        <div class="modal-dialog" style="width: 750px;" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">Расширенный фильтр</h4>
                </div>
                <div class="modal-body">
                    <form name="extFilterForm" method="POST" id="extFilterForm" action="javascript: void(null);">
                        <input type="hidden" name="page" value="<%=(Master.curBase+curTable+Master.curPage)%>">
                        <input type="hidden" name="act" value="setextfilter">
                        <div class='input-group' style="width: 100%">

                            <%--Код ЭА--%>
                            <div class="row">
                                <div class="col-sm-2 pr0" >
                                    <label for="extfId" class="extfLabel">Код ЭА</label>
                                </div>
                                <div class="col-sm-5 pr0">
                                    <input id="extfId" name="extfId" type="text" class="form-control input-sm" value="">
                                </div>
                                <div class="col-sm-3 pr0">
                                    <select class="form-control input-sm" id="extfIdCond" name="extfIdCond">
                                        <option value="=">Равно</option>
                                        <option value="<>">Не равно</option>
                                        <option value=">">Больше</option>
                                        <option value="<">Меньше</option>
                                    </select>
                                </div>
                                <div class="col-sm-2">
                                    <button type="button" class="btn btn-default btn-sm" id="extfClearId" onclick="$('#extfId').val('');$('#extfIdCond').val('=');">Очистить</button>
                                </div>
                            </div>

                            

                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" onclick="$('button[id^=\'extfClear\']').click();SendExtFilterValue();">Сбросить</button>
                    <button type="button" class="btn btn-primary" onclick="SendExtFilterValue()">Применить</button>
                </div>
            </div>
        </div>
    </div>

    <div id="EditDialog" class="modal" tabindex="-1" role="dialog" aria-labelledby="myLargeModalLabel">
        <div class="modal-dialog modal-lg" style="width: 1007px;" role="document">
            <div id="EditDialogContent" class="modal-content">
            </div>
        </div>
    </div>

    <div id="FilterBar">
        <form name="FilterForm" method="POST" id="FilterForm" action="javascript: void(null);">

            <input type="hidden" name="page" value="<%=(Master.curBase+curTable+Master.curPage)%>">
            <input type="hidden" name="act" value="setfilter">
            <div class="col-sm-1" style="padding-right: 0px;">
                <div class='input-group' style="width: 100%">
                    <input id="fId" name="fId" type="text" class="form-control input-sm" value="" placeholder="Код ЭА">
                    <span class="input-group-addon">
                        <span id="fIdClear" class="glyphicon glyphicon-remove"></span>
                    </span>
                </div>
            </div>
            <div class="col-sm-2 pr0">
                <div id="scrollable-dropdown-menu-fUser">
                    <div class='input-group' style="width: 100%">
                        <input id="fUser" name="fUser" type="text" class="form-control input-sm" style="width: 100%" value="" placeholder="Оператор">
                        <span class="input-group-addon">
                            <span id="fUserClear" class="glyphicon glyphicon-remove"></span>
                        </span>
                    </div>
                    <input id="fIdUser" name="fIdUser" type="hidden" value="0">
                </div>
            </div>
            <div class="col-sm-2 pr0">
                <div class='input-group' style="width: 100%">
                    <input id="fDocNum" name="fDocNum" type="text" class="form-control input-sm" value="" placeholder="Номер документа">
                    <span class="input-group-addon">
                        <span id="fDocNumClear" class="glyphicon glyphicon-remove"></span>
                    </span>
                </div>
            </div>
            <div class="col-sm-2 pr0">
                <div id="scrollable-dropdown-menu-fDocTree">
                    <div class='input-group' style="width: 100%">
                        <input id="fDocTree" name="fDocTree" type="text" class="form-control input-sm" style="width: 100%" value="" placeholder="Документ">
                        <span class="input-group-addon">
                            <span id="fDocTreeClear" class="glyphicon glyphicon-remove"></span>
                        </span>
                    </div>
                    <input id="fIdDocTree" name="fIdDocTree" type="hidden" value="0">
                </div>
            </div>
            <div class="col-sm-2 pr0">
                <div class="row">
                    <div class="col-sm-6 pr0">
                        <div class='input-group date' style="width: 100%" id='dpDocDateBegin'>
                            <input id="fDocDateBegin" name="fDocDateBegin" type="text" class="form-control input-sm" value="" placeholder="Дата док. c">
                            <span class="input-group-addon">
                                <span class="glyphicon glyphicon-calendar"></span>
                            </span>
                        </div>
                    </div>
                    <div class="col-sm-6" style="padding-left: 5px;">
                        <div class='input-group date' style="width: 100%" id='dpDocDateEnd'>
                            <input id="fDocDateEnd" name="fDocDateEnd" type="text" class="form-control input-sm" value="" placeholder="Дата док. по">
                            <span class="input-group-addon">
                                <span class="glyphicon glyphicon-calendar"></span>
                            </span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-2">
                <div id="scrollable-dropdown-menu-fFrmContr">
                    <div class='input-group' style="width: 100%">
                        <input id="fFrmContr" name="fFrmContr" type="text" class="form-control input-sm" style="width: 100%" value="" placeholder="Контрагент">
                        <span class="input-group-addon">
                            <span id="fFrmContrClear" class="glyphicon glyphicon-remove"></span>
                        </span>
                    </div>
                </div>
                <input id="fIdFrmContr" name="fIdFrmContr" type="hidden" value="0">
            </div>
            <div class="col-sm-1" style="padding-left: 0px;">
                <button type="button" class="btn btn-default btn-sm" onclick="$('#extFilterDialog').modal();">Расширенный фильтр</button>
            </div>
        </form>
    </div>

    <script>
        var editor;

        $(window).bind('resize', function () {
            $('.dataTables_scrollBody').css('height', ($(window).height() - 170) + 'px');
        });

        $(document).ready(function () {

            document.title = '<%=curPageName%>';

            $('#curPageTitle').text('Электронный архив | База: <%=Master.curBaseNameRus%> | Документы | <%=curPageName%>');

            // EDITOR
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

            // DATATABLE
            var table = $('#table').DataTable({
                dom: '<"row top-toolbar"<"col-sm-4"B><"col-sm-4"><"col-sm-4">><"row top-filterbar">Zr<"m"<"v">t><"row"<"col-sm-5"><"col-sm-3"p><"col-sm-4"i>>',
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
                                            window.location = '<%=GetRouteUrl("archive", new { pBase = Master.curBase, pPage = "All" })%>';
                                        },
                                    },
                                    {
                                        text: 'Бухгалтерские документы',
                                        action: function (e, dt, node, config) {
                                            window.location = '<%=GetRouteUrl("archive", new { pBase = Master.curBase, pPage = "Acc" })%>';
                                        },
                                    },
                                    {
                                        text: 'Договоры',
                                        action: function (e, dt, node, config) {
                                            window.location = '<%=GetRouteUrl("archive", new { pBase = Master.curBase, pPage = "Dog" })%>';
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
                ],
                language: {
                    url: '/content/DataTables-1.10.12/js/Russian.json'
                }
            });

            // Заменяем блок фильтра после полной готовности грида
            $('#table').on('init.dt', function () {
                $('#FilterBar').prependTo('.top-filterbar');
                $(window).resize();
            }).dataTable();

            // ROWINFO
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
                            $('#myTab' + row.data().Id + ' a:first').tab('show');
                        }
                    );
                }
            });

            // ROWDBLCLICK
            $('#table').on('dblclick', 'tr', function () {
                var table = $('#table').DataTable();
                var id = table.row(this).id();
                $('#EditDialog').modal();
                $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&curPage=<%=Master.curPage%>&action=edit&curId=' + id + '&_=' + (new Date()).getTime());
            });

            // CONTEXTMENU
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

            // ----------------
            // ----- FILTER----
            // ----------------

            //
            // ID
            //

            $("#fId").bind("keyup", function () {
                $(this).val($(this).val().replace(/[^0-9]+/g, ""));
                ChangeFilterColor($(this));
                SendFilterValue();
            });
            $("#fIdClear").bind("click", function () {
                $("#fId").val("");
                ChangeFilterColor($("#fId"));
                SendFilterValue();
            });

            //
            //User
            //

            var sourcefUser = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.whitespace, queryTokenizer: Bloodhound.tokenizers.whitespace,
                remote: {
                    url: '/Handler/TypeaheadHandler.ashx?t=User&q=%QUERY',
                    wildcard: '%QUERY'
                },
                limit: 30,
            });

            $('#scrollable-dropdown-menu-fUser #fUser').typeahead({
                highlight: true,
                minLength: 1,
            },
            {
                name: 'thfUser',
                display: 'Name',
                highlight: true,
                limit: 30,
                source: sourcefUser,
            });

            $('#fUser').on("typeahead:selected typeahead:autocompleted", function (e, datum) {
                $("#fIdUser").val(datum.ID);
                $("#fUser").parent().find('input').addClass("filter-active");
                SendFilterValue();
            });

            $("#fUser").bind("change", function () {
                if ($(this).val().trim() == '')
                    $("#fUserClear").click();
            });

            $("#fUserClear").bind("click", function () {
                $("#fIdUser").val(0);
                $("#fUser").val('');
                $("#fUser").parent().find('input').removeClass("filter-active");
                SendFilterValue();
            });

            //
            //DocNum
            //

            $("#fDocNum").bind("keyup", function () {
                ChangeFilterColor($("#fDocNum"));
                SendFilterValue();
            });
            $("#fDocNumClear").bind("click", function () {
                $("#fDocNum").val("");
                ChangeFilterColor($("#fDocNum"));
                SendFilterValue();
            });

            //
            //Doctree
            //

            var sourcefDocTree = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.whitespace, queryTokenizer: Bloodhound.tokenizers.whitespace,
                remote: {
                    url: '/Handler/TypeaheadHandler.ashx?t=DocTree&q=%QUERY',
                    wildcard: '%QUERY'
                },
                limit: 30,
            });

            $('#scrollable-dropdown-menu-fDocTree #fDocTree').typeahead({
                highlight: true,
                minLength: 1,
            },
            {
                name: 'thfDocTree',
                display: 'Name',
                highlight: true,
                limit: 10,
                source: sourcefDocTree,
            });

            $('#fDocTree').on("typeahead:selected typeahead:autocompleted", function (e, datum) {
                $("#fIdDocTree").val(datum.ID);
                $("#fDocTree").addClass("filter-active");
                SendFilterValue();
            });

            $("#fDocTree").bind("change", function () {
                if ($(this).val().trim() == '')
                    $("#fDocTreeClear").click();
            });

            $("#fDocTreeClear").bind("click", function () {
                $("#fIdDocTree").val(0);
                $("#fDocTree").val('');
                $("#fDocTree").parent().find('input').removeClass("filter-active");
                SendFilterValue();
            });

            //
            // DocDate
            //

            $('#fDocDateBegin,#fDocDateEnd').mask('99.99.9999', { placeholder: 'дд.мм.гггг' });
            $('#dpDocDateBegin,#dpDocDateEnd').datetimepicker({ locale: 'ru', useCurrent: false, format: 'DD.MM.YYYY', });
            $("#fDocDateBegin").bind("blur", function () {
                ChangeFilterColor($(this));
                SendFilterValue();
            });
            $("#fDocDateEnd").bind("blur", function () {
                ChangeFilterColor($(this));
                SendFilterValue();
            });

            //
            //Frm
            //

            var sourcefFrmContr = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.whitespace, queryTokenizer: Bloodhound.tokenizers.whitespace,
                remote: {
                    url: '/Handler/TypeaheadHandler.ashx?t=Frm&q=%QUERY',
                    wildcard: '%QUERY'
                },
                limit: 30,
            });

            $('#scrollable-dropdown-menu-fFrmContr #fFrmContr').typeahead({
                highlight: true,
                minLength: 1,
            },
            {
                name: 'thfFrmContr',
                display: 'Name',
                highlight: true,
                limit: 30,
                source: sourcefFrmContr,
            });

            $('#fFrmContr').on("typeahead:selected typeahead:autocompleted", function (e, datum) {
                $("#fIdFrmContr").val(datum.ID);
                $("#fFrmContr").addClass("filter-active");
                SendFilterValue();
            });

            $("#fFrmContr").bind("change", function () {
                if ($(this).val().trim() == '')
                    $("#fFrmContrClear").click();
            });

            $("#fFrmContrClear").bind("click", function () {
                $("#fIdFrmContr").val(0);
                $("#fFrmContr").val('');
                $("#fFrmContr").parent().find('input').removeClass("filter-active");
                SendFilterValue();
            });

            // ----------------
            // ---EXT FILTER---
            // ----------------

            //
            // Id
            //

            $("#extfId").bind("keyup", function () {
                $(this).val($(this).val().replace(/[^0-9]+/g, ""));
            });

        });

        function ChangeFilterColor(el) {
            if (el.val().trim() == '') {
                el.removeClass("filter-active");
                console.log(el.val() + " no");
            }
            else {
                el.addClass("filter-active");
                console.log(el.val() + " yes");
            }
        }

        function SendFilterValue() {
            var msg = $('#FilterForm').serialize();
            $.ajax({
                type: 'POST',
                url: '/Handler/SessionHandler.ashx',
                data: msg,
                success: function (data) {
                    $('#table').DataTable().draw();
                },
            });
        }
        function SendExtFilterValue() {
            var msg = $('#extFilterForm').serialize();
            $.ajax({
                type: 'POST',
                url: '/Handler/SessionHandler.ashx',
                data: msg,
                success: function (data) {
                    $('#table').DataTable().draw();
                },
            });
        }
    </script>
</asp:Content>