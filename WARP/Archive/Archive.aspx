<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="WARP.Archive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">

    <table id="table_id" class="table table-striped table-bordered" style="table-layout: fixed; width: 100%">
        <%--;word-wrap:break-word;--%>
        <thead>
            <tr>
                <%=htmlTableColumns%>
            </tr>
        </thead>
    </table>

    <script>
        $(window).bind('resize', function () {
            var h = $(window).height();
            $('.dataTables_scrollBody').css('height', (h - 125) + 'px');
        });
        $(document).ready(function () {
            document.title = '<%=documentTitle%>';
            //

            $('#table_id').DataTable({
                dom: 'ZBrt<"row"<"col-sm-5"i><"col-sm-7"p>>',
                "processing": true,
                "serverSide": true,
                "ajax": "/Handler.ashx?table=archive_<%=curArchivePage.ToString()%>",
                "columns": [
                    <%=jsTableColumns%>
                ],

                autoWidth: false,
                select: true,
                colReorder: { realtime: false },
                "colResize": {
                    "tableWidthFixed": true
                },
                stateSave: true,
                scrollY: ($(window).height() - 125) + "px",
                scrollX: true,
                scrollCollapse: false,
                lengthMenu: [
                    [20, 50, 100, 200],
                    ['20 строк', '50 строк', '100 строк', '200 строк']
                ],
                buttons: [

                    {
                        extend: 'collection',
                        text: 'Меню',
                        buttons: [
                            {
                                text: 'Алерт',
                                action: function (e, dt, node, config) {
                                    alert('dfg');
                                }
                            }
                        ]
                    },
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
                        ]
                    },
                    {
                        extend: 'collection',
                        text: 'Экспорт',
                        buttons: [
                            'copyHtml5',
                            'excelHtml5',
                            'csvHtml5',
                            'pdfHtml5',
                        ]
                    },

                ],
                language: {
                    url: '/content/DataTables-1.10.12/js/Russian.json'
                }

            });

            $(window).resize();

        });
    </script>
</asp:Content>