<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ArchiveVer.aspx.cs" Inherits="WARP.ArchiveVer" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">

    <table id="table" class="table table-striped table-bordered table-condensed" style="table-layout: fixed; width: 100%">
        <thead>
            <tr>
                <%=tableArchive.GenerateHtmlTable()%>
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
            $('.dataTables_scrollBody').css('height', ($(window).height() - $('.top-toolbar').height()  - 90) + 'px');
        });

        $(document).ready(function () {

            document.title = 'Предыдущие версии документа';

            $('#ddSection').html(document.title+'<b class="caret"></b>');

            // DATATABLE
            var table = $('#table').DataTable({
                dom: '<"row top-toolbar"<"col-sm-7"B><"col-sm-5"<"row"<"col-sm-7"p><"col-sm-5"i>>>><"top-filterbar">Zrt',
                rowId: 'IdVer',
                order: [[ 1, "desc" ]],
                processing: true,
                serverSide: true,
                ajax: "/Handler/GetDataHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&curPage=<%=Master.curPage%>&curId=<%=curId%>&showVer=1",
                columns: [
<%=tableArchive.GenerateJSTableColumns()%>
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
                        extend: 'collection',
                        text: 'Настройка таблицы:',
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
                //$('#FilterBar').prependTo('.top-filterbar');
                $(window).resize();
            }).dataTable();

            // ROWINFO
            var detailRows = [];


            $('#table tbody').on('click', 'tr td.details-control', function () {
                var tr = $(this).closest('tr');
                var row = table.row(tr);

                if (row.child.isShown()) {
                    tr.removeClass('details');
                    row.child.hide();
                }
                else {
                    tr.addClass('details');
                    row.child('<div id="RowInfo' + row.data().IdVer + '">Загрузка..</div>').show();
                    $('#RowInfo' + row.data().IdVer).load(
                        '/Handler/InfoButtonHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&curPage=<%=Master.curPage%>&showVer=1&curId=' + row.data().IdVer + '&_=' + (new Date()).getTime(), null,
                        function () {
                            $('#myTab' + row.data().IdVer + ' a:first').tab('show');
                        }
                    );
                }
            });

            // ROWDBLCLICK
            $('#table').on('dblclick', 'tr', function () {
                var table = $('#table').DataTable();
                var id = table.row(this).id();
                if (id > 0) {
                    $('#EditDialog').modal();
                    $('#EditDialogContent').load('/Handler/EditDialogHandler.ashx?curBase=<%=Master.curBase%>&curTable=<%=curTable%>&curPage=<%=Master.curPage%>&showVer=1&action=edit&curId=' + id + '&_=' + (new Date()).getTime());
                }
            });

            
        });

    </script>
</asp:Content>