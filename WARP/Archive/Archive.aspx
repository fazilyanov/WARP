﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="WARP.Archive" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">
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
                    {
                        label: "Номер документа:",
                        name: "NumDoc",
                    },
                    {
                        label: "Примечание:",
                        name: "Prim"
                    },
                    {
                        label: "Содержание:",
                        name: "DocContent"
                    },
                ]
            });
            //$('#table_id').on('click', 'tbody td:not(:first-child)', function (e) {
            //    editor.inline(this);
            //});

            $('#table_id').DataTable({
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
                    { extend: 'create', editor: editor, className: 'btn-sm', },
                    { extend: 'edit', editor: editor, className: 'btn-sm', },
                    { extend: 'remove', editor: editor, className: 'btn-sm', },
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