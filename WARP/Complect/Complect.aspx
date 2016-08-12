<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Complect.aspx.cs" Inherits="WARP.Complect" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">

    <table id="table" class="table table-striped table-bordered table-condensed" style="table-layout: fixed; width: 100%">
        <thead>
            <tr>
                <%=tableComplect.GenerateHtmlTable()%>
            </tr>
        </thead>
    </table>

    
    <div id="EditDialogDetail" class="modal" tabindex="-1" role="dialog" style="z-index:1080;">
        <div class="modal-dialog  modal-vertical-centered" style="width: 685px;" role="document">
            <div id="EditDialogDetailContent" class="modal-content">
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

            <input type="hidden" name="page" value="<%=(Master.curBase + curTable)%>">
            <input type="hidden" name="act" value="setfilter">
            <div class="row">

                <!-- Id -->
                <div class="col-sm-1 pr0">
                    <div class='input-group' style="width: 100%">
                        <input id="fId" name="fId" type="text" class="form-control input-sm" value="" placeholder="Id">
                        <div class="input-group-btn" style="width: 1%">
                            <button type="button" id="fIdExt" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-filter"></span>
                            </button>
                            <button type="button" id="fIdClear" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-remove"></span>
                            </button>
                        </div>
                    </div>
                </div>

                <%--<!-- Barcode -->
                <div class="col-sm-1 pr0">
                    <div class='input-group' style="width: 100%">
                        <input id="fBarcode" name="fBarcode" type="text" class="form-control input-sm" value="" placeholder="Штрихкод">
                        <div class="input-group-btn" style="width: 1%">
                            <button type="button" id="fBarcodeExt" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-filter"></span>
                            </button>
                            <button type="button" id="fBarcodeClear" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-remove"></span>
                            </button>
                        </div>
                    </div>
                </div>

                <!-- DocDate -->
                <div class="col-sm-1 pr0">
                    <div class='input-group date' style="width: 100%" id='dpDocDateBegin'>
                        <input id="fDocDate" name="fDocDate" type="text" class="form-control input-sm" value="" placeholder="Дата документа">
                        <div class="input-group-btn" style="width: 1%">
                            <button type="button" id="fDocDateExt" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-filter"></span>
                            </button>
                            <button type="button" id="fDocDateClear" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-remove"></span>
                            </button>
                        </div>
                    </div>
                </div>

                <!-- DocNum -->
                <div class="col-sm-1 pr0">
                    <div class='input-group' style="width: 100%">
                        <input id="fDocNum" name="fDocNum" type="text" class="form-control input-sm" value="" placeholder="Номер документа">
                        <div class="input-group-btn" style="width: 1%">
                            <button type="button" id="fDocNumExt" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-filter"></span>
                            </button>
                            <button type="button" id="fDocNumClear" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-remove"></span>
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Parent -->
                <div class="col-sm-2 pr0">
                    <div id="scrollable-dropdown-menu-fParent">
                        <div class='input-group' style="width: 100%">
                            <input id="fParent" name="fParent" type="text" class="form-control input-sm" style="width: 100%" value="" placeholder="Договор">
                            <div class="input-group-btn" style="width: 1%">
                                <button type="button" id="fParentExt" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-filter"></span>
                                </button>
                                <button type="button" id="fParentClear" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-remove"></span>
                                </button>
                            </div>
                        </div>
                        <input id="fIdParent" name="fIdParent" type="hidden" value="">
                    </div>
                </div>

                <!-- DocTree -->
                <div class="col-sm-2 pr0">
                    <div id="scrollable-dropdown-menu-fDocTree">
                        <div class='input-group' style="width: 100%">
                            <input id="fDocTree" name="fDocTree" type="text" class="form-control input-sm" style="width: 100%" value="" placeholder="Документ">
                            <div class="input-group-btn" style="width: 1%">
                                <button type="button" id="fDocTreeExt" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-filter"></span>
                                </button>
                                <button type="button" id="fDocTreeClear" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-remove"></span>
                                </button>
                            </div>
                        </div>
                        <input id="fIdDocTree" name="fIdDocTree" type="hidden" value="">
                    </div>
                </div>

                <!-- FrmContr -->
                <div class="col-sm-2 pr0">
                    <div id="scrollable-dropdown-menu-fFrmContr">
                        <div class='input-group' style="width: 100%">
                            <input id="fFrmContr" name="fFrmContr" type="text" class="form-control input-sm" style="width: 100%" value="" placeholder="Контрагент">
                            <div class="input-group-btn" style="width: 1%">
                                <button type="button" id="fFrmContrExt" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-filter"></span>
                                </button>
                                <button type="button" id="fFrmContrClear" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-remove"></span>
                                </button>
                            </div>
                        </div>
                    </div>
                    <input id="fIdFrmContr" name="fIdFrmContr" type="hidden" value="">
                </div>

                <!-- User -->
                <div class="col-sm-2" style="width: 15%;">
                    <div id="scrollable-dropdown-menu-fUser">
                        <div class='input-group' style="width: 100%">
                            <input id="fUser" name="fUser" type="text" class="form-control input-sm" style="width: 100%" value="" placeholder="Оператор">
                            <div class="input-group-btn" style="width: 1%">
                                <button type="button" id="fUserClear" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-remove"></span>
                                </button>
                            </div>
                        </div>
                        <input id="fIdUser" name="fIdUser" type="hidden" value="">
                    </div>
                </div>
                <div class="col-sm-2" style="width: 1%; padding: 5px 0px;">
                    <a href="#" id="showAddFilter" onclick="$(this).hide();$('#hideAddFilter').show();$('#addFilter').fadeIn('fast',function(){$(window).resize();});" title="Показать остальные фильтры"><span class="glyphicon glyphicon-chevron-down"></span></a>
                    <a href="#" id="hideAddFilter" onclick="$(this).hide();$('#showAddFilter').show();$('#addFilter').fadeOut('fast',function(){$(window).resize();});" style="display: none" title="Скрыть остальные фильтры"><span class="glyphicon glyphicon-chevron-up"></span></a>
                </div>--%>
            </div>
<%--
            <!-- Расширенный -->
            <div id="addFilter" style="display: none;">
                <div class="row" style="padding-top: 5px">

                    <!-- DateUpd -->
                    <div class="col-sm-2 pr0">
                        <div class="row">
                            <div class="col-sm-5 pr0" style="width:43%">
                                <div class='input-group' style="width: 100%">
                                    <input id="fDateUpdBegin" name="fDateUpdBegin" type="text" class="form-control input-sm" value="" placeholder="Дата ред. с">
                                </div>
                            </div>
                            <div class="col-sm-7" style="padding-left:0px;width:57%">
                                <div class='input-group date' style="width: 100%" id='dpDateUpdBegin'>
                                    <input id="fDateUpdEnd" name="fDateUpdEnd" type="text" class="form-control input-sm" value="" placeholder="Дата ред. по" 
                                        style="border-left: 0px;border-top-left-radius: 0;border-bottom-left-radius: 0;">
                                    <div class="input-group-btn" style="width: 1%">
                                        <button type="button" id="fDateUpdExt" class="btn btn-default btn-sm fbtn">
                                            <span class="glyphicon glyphicon-filter"></span>
                                        </button>
                                        <button type="button" id="fDateUpdClear" class="btn btn-default btn-sm fbtn">
                                            <span class="glyphicon glyphicon-remove"></span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- DocContent -->
                    <div class="col-sm-2 pr0">
                        <div class='input-group' style="width: 100%">
                            <input id="fDocContent" name="fDocContent" type="text" class="form-control input-sm" value="" placeholder="Содержание">
                            <div class="input-group-btn" style="width: 1%">
                                <button type="button" id="fDocContentExt" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-filter"></span>
                                </button>
                                <button type="button" id="fDocContentClear" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-remove"></span>
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- Status -->
                    <div class="col-sm-1 pr0">
                        <div id="scrollable-dropdown-menu-fStatus">
                            <div class='input-group' style="width: 100%">
                                <input id="fStatus" name="fStatus" type="text" class="form-control input-sm" style="width: 100%" value="" placeholder="Статус">
                                <div class="input-group-btn" style="width: 1%">
                                    <button type="button" id="fStatusExt" class="btn btn-default btn-sm fbtn">
                                        <span class="glyphicon glyphicon-filter"></span>
                                    </button>
                                    <button type="button" id="fStatusClear" class="btn btn-default btn-sm fbtn">
                                        <span class="glyphicon glyphicon-remove"></span>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <input id="fIdStatus" name="fIdStatus" type="hidden" value="">
                    </div>

                    <!-- Source -->
                    <div class="col-sm-1 pr0">
                        <div id="scrollable-dropdown-menu-fSource">
                            <div class='input-group' style="width: 100%">
                                <input id="fSource" name="fSource" type="text" class="form-control input-sm" style="width: 100%" value="" placeholder="Источник">
                                <div class="input-group-btn" style="width: 1%">
                                    <button type="button" id="fSourceExt" class="btn btn-default btn-sm fbtn">
                                        <span class="glyphicon glyphicon-filter"></span>
                                    </button>
                                    <button type="button" id="fSourceClear" class="btn btn-default btn-sm fbtn">
                                        <span class="glyphicon glyphicon-remove"></span>
                                    </button>
                                </div>
                            </div>
                        </div>
                        <input id="fIdSource" name="fIdSource" type="hidden" value="">
                    </div>


                    <!-- DateTrans -->
                    <div class="col-sm-1 pr0">
                        <div class='input-group' style="width: 100%">
                            <input id="fDateTrans" name="fDateTrans" type="text" class="form-control input-sm" value="" placeholder="Дата передачи">
                            <div class="input-group-btn" style="width: 1%">
                                <button type="button" id="fDateTransExt" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-filter"></span>
                                </button>
                                <button type="button" id="fDateTransClear" class="btn btn-default btn-sm fbtn">
                                    <span class="glyphicon glyphicon-remove"></span>
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- Summ -->
                    <div class="col-sm-1 pr0">
                    <div class='input-group' style="width: 100%">
                        <input id="fSumm" name="fSumm" type="text" class="form-control input-sm" value="" placeholder="Сумма">
                        <div class="input-group-btn" style="width: 1%">
                            <button type="button" id="fSummExt" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-filter"></span>
                            </button>
                            <button type="button" id="fSummClear" class="btn btn-default btn-sm fbtn">
                                <span class="glyphicon glyphicon-remove"></span>
                            </button>
                        </div>
                    </div>
                </div>
                </div>
            </div>--%>
        </form>
    </div>

    <script>
        var editor;
        var table;
        var tableDetail;

        $(window).bind('resize', function () {
            $('#table_wrapper .dataTables_scrollBody').css('height', ($(window).height() - $('.top-toolbar').height() - $('.top-filterbar').height() - 90) + 'px');
        });

        $(document).ready(function () {

            document.title = 'Комплекты';

            $('#ddSection').html('Комплекты <b class="caret"></b>');

            
            // DATATABLE
            table = $('#table').DataTable({
                dom: '<"row top-toolbar"<"col-sm-7"B><"col-sm-5"<"row"<"col-sm-7"p><"col-sm-5"i>>>><"top-filterbar">Zrt',
                rowId: 'Id',
                processing: true,
                serverSide: true,
                ajax: "/Handler/GetDataHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>",
                columns: [
<%=tableComplect.GenerateJSTableColumns()%>
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
                            $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&action=create&curId=0&_=' + (new Date()).getTime());
                        },
                        key: "n",
                        className: "btn-sm",
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


           <%-- $('#table tbody').on('click', 'tr td.details-control', function () {
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
                        '/Handler/InfoButtonHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&curId=' + row.data().Id + '&_=' + (new Date()).getTime(), null,
                        function () {
                            $('#myTab' + row.data().Id + ' a:first').tab('show');
                        }
                    );
                }
            });--%>

            // ROWDBLCLICK
            $('#table').on('dblclick', 'tr', function () {
                var table = $('#table').DataTable();
                var id = table.row(this).id();
                if (id > 0) {
                    $('#EditDialog').modal();
                    $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&action=edit&curId=' + id + '&_=' + (new Date()).getTime());
                }
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