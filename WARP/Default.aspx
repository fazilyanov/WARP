<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WARP.Default" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">
    <script type="text/javascript">
        jQuery(document).ready(function () {
        <%
        if (Master.curBaseName == string.Empty)
        {
        %>
            $('.navbar-right > li').first().addClass('open');
        <%
        }
        else
        {
        %>
            $('.navbar-right > li').first().next().addClass('open');
            $('#curPageTitle').text('Электронный архив / База: <%=Master.curBaseNameRus%>');
        <%
        }
        %>

        });
    </script>
</asp:Content>